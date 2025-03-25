using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepoLayer;
using ModelLayer;

namespace BusinessLayer
{
    public class UserLogin : IUserLogin
    {
        private readonly UserDBContext context;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;
        private readonly IJwtService jwtService;
        private readonly IUserRepository repository;

        public UserLogin(UserDBContext context,IConfiguration configuration,IEmailService emailService,IJwtService jwtService,IUserRepository repository)
        {
            this.context = context;
            this.configuration = configuration;
            this.emailService = emailService;
            this.jwtService = jwtService;
            this.repository = repository;
        }

        public void AddUser(UserModel users)
        {
            var isDuplicateUser = context.Users.Any(e => e.Email == users.Email);
            if (isDuplicateUser)
            {
                throw new InvalidOperationException("Email already exists!");
            }
            UserRegistration user = new UserRegistration
            {
                FirstName=users.FirstName,
                LastName=users.LastName,
                Email=users.Email,
                Password=BCrypt.Net.BCrypt.HashPassword(users.Password)
            };
            context.Users.Add(user);
            context.SaveChanges();
        }

        public List<UserRegistration> GetAllUsers()
        {
            return context.Users.ToList();
        }

        public UserRegistration LoginUser(string email,string password)
        {
            var user = repository.GetUserByEmail(email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return user; // Return user instead of void
            }
            return null; // Return null if authentication fails
        }

        public bool ForgotPassword(string email)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                throw new Exception("User not found!");

            // Generate Reset Token
            string resetToken = jwtService.GenerateResetToken(email);

            // Send Email
            string resetLink = $"https://localhost:4200/reset-password?token={resetToken}";
            string subject = "Reset Password";
            string body = $"Click the link to reset your password: <a href='{resetLink}'>Reset Password</a>";

            return emailService.SendEmail(email, subject, body);
        }

        public bool SendResetEmail(string toEmail)
        {
            string resetToken = jwtService.GenerateResetToken(toEmail);

            string resetLink = $"https://localhost:4200/reset-password?token={resetToken}";

            string subject = "Password Reset Request";
            string body = $@"
                <p>Hello,</p>
                <p>Click the link below to reset your password:</p>
                <a href='{resetLink}'>Reset Password</a>
                <p>If you did not request this, please ignore this email.</p>";

            return emailService.SendEmail(toEmail, subject, body);
        }

        public bool ResetPassword(string token, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
                throw new Exception("Passwords do not match!");

            Console.WriteLine("Received Token : " + token);
            string email = jwtService.ValidateResetToken(token);
            Console.WriteLine("Extracted Email: " + email);
            if (email == null)
                throw new Exception("Invalid or expired token!");

            var user = repository.GetUserByEmail(email);
            if (user == null)
                throw new Exception("User not found!");

            // Update password
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            repository.UpdateUser(user);

            return true;
        }
    }
}