using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using RepoLayer;

namespace BusinessLayer
{
    public interface IJwtService
    {
        string GenerateToken(UserRegistration user, string jwtType);
        string GenerateResetToken(string email);  // Add this
        string ValidateResetToken(string token);  // Add this
        ClaimsPrincipal ValidateToken(string token);
        int GetUserIdFromToken(string token);
    }
}