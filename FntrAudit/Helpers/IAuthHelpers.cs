using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FntrAudit.Helpers
{
    public interface IAuthHelpers
    {
        Task<string> GenerateRandomStringAsync();
    }
}
