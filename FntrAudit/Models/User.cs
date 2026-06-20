using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FntrAudit.Models
{
    public class User : BaseEntity
    {
        public int? idSociete { get; set; }
        public string? nom { get; set; }
        public string? prenom { get; set; }
        public int? societeUser_Id { get; set; }
        public int? role { get; set; }
        public string? mail { get; set; }
        public string? pass { get; set; }
        public string? identifiant { get; set; }
        public string? accessFailedCount { get; set; }
        public string? concurrencyStamp { get; set; }
        public string? email { get; set; }
        public string? emailConfirmed { get; set; }
        public string? lockoutEnabled { get; set; }
        public string? lockoutEnd { get; set; }
        public string? normalizedEmail { get; set; }
        public string? normalizedUserName { get; set; }
        public string? passwordHash { get; set; }
        public string? phoneNumber { get; set; }
        public string? phoneNumberConfirmed { get; set; }
        public string? roleId { get; set; }
        public string? securityStamp { get; set; }
        public string? twoFactorEnabled { get; set; }
        public string? userName { get; set; }
        public bool hasNewPassword { get; set; }
        public string? titre1 { get; set; }
        public string? titre2 { get; set; }
        public string? titre3 { get; set; }
        public string? titre4 { get; set; }
    }
}
