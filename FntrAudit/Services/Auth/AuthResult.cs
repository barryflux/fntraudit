using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FntrAudit.Services.Auth
{
    public class AuthResult
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }
        public Models.User? User { get; }

        private AuthResult(bool isSuccess, Models.User? user, string? errorMessage)
        {
            IsSuccess = isSuccess;
            User = user;
            ErrorMessage = errorMessage;
        }

        public static AuthResult Success(Models.User user) => new(true, user, null);

        public static AuthResult Failure(string message) => new(false, null, message);
    }
}
