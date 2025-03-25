using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepoLayer;

namespace BusinessLayer
{
    public interface IAuthService
    {
        string Login(string email, string password);
        string Register(UserRegistration newUser);
        string ValidateToken(string token, string jwtType);
        UserRegistration AuthenticateUser(string email, string password);
        string GenerateJwtToken(UserRegistration user, string jwtType);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository repository;
        private readonly IJwtService service;

        public AuthService(IUserRepository repository, IJwtService service)
        {
            this.repository = repository;
            this.service = service;
        }

        public UserRegistration AuthenticateUser(string email, string password)
        {
            var user = repository.GetUserByEmail(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }
            return user;
        }

        public string GenerateJwtToken(UserRegistration user, string jwtType)
        {
            return service.GenerateToken(user, jwtType);
        }

        public string Login(string email, string password)
        {
            var user = repository.GetUserByEmail(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new Exception("Invalid email or password.");
            }
            return service.GenerateToken(user, "auth");
        }

        public string Register(UserRegistration newUser)
        {
            var existingUser = repository.GetUserByEmail(newUser.Email);
            if (existingUser != null)
            {
                throw new Exception("User already exists.");
            }
            newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
            repository.AddUser(newUser);
            return service.GenerateToken(newUser, "auth");
        }

        public string ValidateToken(string token, string jwtType)
        {
            var claimsPrincipal = service.ValidateToken(token);
            return claimsPrincipal != null ? "Valid Token" : "Invalid Token";
        }
    }
}