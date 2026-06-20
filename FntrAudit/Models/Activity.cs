using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FntrAudit.Models
{
   public class Activity : BaseEntity
   {
        public bool isOk {  get; set; }
        public int auditId { get; set; }
    }
}
