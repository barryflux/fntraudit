using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FntrAudit.Models
{
    public class Client : BaseEntity
    {
        public string? personneSollicitante { get; set; }

        public string? personneInterrogee { get; set; }
        public string? fonction { get; set; }
        public string? statut { get; set; }
        public string? capital { get; set; }

        public string? raisonSociale { get; set; }
        public string? siret { get; set; }
        public string? adresse { get; set; }

        public string? naf { get; set; }
        public string? formeJuridique { get; set; }

        public string? caAnnuel { get; set; }
        public string? logo { get; set; }
        public string? historique { get; set; }
        public string? etabSecondaire { get; set; }

        public int? idActivite { get; set; }

        public int? societeUser_Id { get; set; }

        public string? auditEncours { get; set; }
        public string? activite { get; set; }

        public bool has11SalOrMore { get; set; }
        public bool has50SalOrMore { get; set; }
        public bool has300SalOrMore { get; set; }
        public bool has1000SalOrMore { get; set; }
        public bool has1SalOrMore { get; set; }
        public bool hasWarningRequiredDoc { get; set; }

        public string? effectif { get; set; }
        public string? nombreLicence { get; set; }
        public string? nbreVehiculeMoteur { get; set; }


        public bool pvCarence { get; set; }
        public bool cse { get; set; }
        public byte[]? picture { get; set; }
        public bool isVoyageur { get; set; }
        public bool isTransport { get; set; }

        public string? email { get; set; }
    }
}
