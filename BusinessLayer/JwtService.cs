using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepoLayer;

namespace BusinessLayer
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration configuration;
        private readonly IUserRepository repository;

        public JwtService(IConfiguration configuration,IUserRepository repository)
        {
            this.configuration = configuration;
            this.repository = repository;
        }

        // Method to Generate JWT Token
        public string GenerateToken(UserRegistration user, string jwtType)
        {
            var key = Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // Store userId as string
                new Claim("JwtType", jwtType)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(configuration["JwtSettings:ExpiryMinutes"])),
                Issuer = configuration["JwtSettings:Issuer"],
                Audience = configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateResetToken(string email)
        {
            var user = repository.GetUserByEmail(email);
            Console.WriteLine($"User email with id {user.UserId} : {user.Email}");
            if (user == null)
                throw new Exception("User not found!");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["JwtSettings:ResetKey"]);

            var now = DateTime.UtcNow;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email) }),
                NotBefore = now,
                Expires = DateTime.UtcNow.AddHours(2),  // Expiry must be later than NotBefore
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string ValidateResetToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:ResetKey"]));

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false, // Change to true if using issuer
                    ValidateAudience = false, // Change to true if using audience
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var principalClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                return principalClaim;
            }
            catch (SecurityTokenExpiredException)
            {
                return "Token has expired!";
            }
            catch (Exception ex)
            {
                return "Token validation error: " + ex.Message;
            }
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;
                var jwtType = jwtToken?.Claims.FirstOrDefault(c => c.Type == "JwtType")?.Value;

                if (jwtType != "Auth")
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public int GetUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");

            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}