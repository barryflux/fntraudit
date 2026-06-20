using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FntrAudit.Models
{
    public class Logo : BaseEntity
    {
        public byte[]? Logolo { get; set; }
        public bool? IsUsed { get; set; }
   
    }
}
