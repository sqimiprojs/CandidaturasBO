using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;

namespace Candidaturas_BO
{
    public class ADAuthorization
    {
        public static Boolean ADAuthenticate(string userName, string password)
        {
            // set up domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, BOConstants.AdDomain, BOConstants.AdDomainDC, BOConstants.AdDomainUser, BOConstants.AdDomainPassword);

            //validate credentials
            if(ctx.ValidateCredentials(userName, password))
            {
                // find current user
                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, userName);

                // find the group in question
                GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, BOConstants.AdGroup);

                if (user != null)
                {
                    // check if user is member of that group
                    if (user.IsMemberOf(group))
                    {
                        HttpContext.Current.Session["userName"] = userName;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static Boolean ADAuthenticate()
        {
            // set up domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, BOConstants.AdDomain, BOConstants.AdDomainDC, BOConstants.AdDomainUser, BOConstants.AdDomainPassword);

            // find current user
            string Name = String.Empty;

            if(HttpContext.Current.Session["userName"] != null)
            {
                Name = HttpContext.Current.Session["userName"].ToString();
            }
            else
            {
                Name = System.Web.HttpContext.Current.User.Identity.Name;
            }

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, Name);

            // find the group in question
            GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, BOConstants.AdGroup);

            if (user != null)
            {
                // check if user is member of that group
                if (user.IsMemberOf(group))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}