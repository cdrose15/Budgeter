using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Budgeter.Models.CodeFirst;

namespace Budgeter.Models
{
    public static class Extentions
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static string GetHouseholdId(this IIdentity user)
        {
            var claimsIdentity = (ClaimsIdentity)user;
            var HouseholdClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "HouseholdId");

            if (HouseholdClaim != null)
                return HouseholdClaim.Value;
            else
                return null;
        }

        public static bool IsInHousehold(this IIdentity user)
        {
            var cUser = (ClaimsIdentity)user;
            var hid = cUser.Claims.FirstOrDefault(c => c.Type == "HouseholdId");
            return (hid != null && !string.IsNullOrWhiteSpace(hid.Value));
        }

        public static string GetName(this IIdentity user)
        {
            var claimuser = (ClaimsIdentity)user;
            var NameClaim = claimuser.Claims.FirstOrDefault(c => c.Type == "Name");
            if (NameClaim != null)
            {
                return NameClaim.Value;
            }
            else
            {
                return null;
            }
        }

        public static async Task RefreshAuthentication(this HttpContextBase context, ApplicationUser user)
        {
            context.GetOwinContext().Authentication.SignOut();
            await context.GetOwinContext().Get<ApplicationSignInManager>().SignInAsync(user, isPersistent: false, rememberBrowser: false);

        }

    }

}