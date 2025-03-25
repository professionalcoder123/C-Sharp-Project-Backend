using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepoLayer;
using ModelLayer;

namespace BusinessLayer
{
    public interface IUserLogin
    {
        void AddUser(UserModel user);

        List<UserRegistration> GetAllUsers();

        UserRegistration LoginUser(string email,string password);

        bool ForgotPassword(string email);

        bool SendResetEmail(string toEmail);

        bool ResetPassword(string token, string newPassword,string confirmPassword);
    }
}
