using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FntrAudit.Models
{
   public class SocieteUser : BaseEntity
    {
        public string? Logo { get; set; }
        public string? Logo1 { get; set; }
        public string? Adresse { get; set; }
        public string? Service { get; set; }
        public string? Siret { get; set; }
        public string? Ape { get; set; }
        public string? Mail { get; set; }
        public string? Telephone { get; set; }
        public string? Fax { get; set; }
        public string? TemplatePath { get; set; }
    }
}
