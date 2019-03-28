using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Candidaturas_BO
{
    public static class BOConstants
    {       
        //Domínio da Active Directory
        public const string AdDomain = "sqimi.net";
        public const string AdDomainDC = "DC=sqimi,DC=net";

        //Utilizador admin da Active Directory
        public const string AdDomainUser = "Administrator";

        //Password do utilizador admin da Active Directory
        public const string AdDomainPassword = "Sqimi2018";

        //Grupo da Active Directory que tem permissões de aceder ao BO
        public const string AdGroup = "Candidaturas";
        public const string AdAdminGroup = "CandidaturasAdmin";
    }
}