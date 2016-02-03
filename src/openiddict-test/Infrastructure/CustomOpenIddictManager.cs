using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using openiddicttest.Models;
using OpenIddict.Models;
using System;

namespace openiddicttest
{
    public class CustomOpenIddictManager : OpenIddict.OpenIddictManager<ApplicationUser, Application>
    {
        public CustomOpenIddictManager(IServiceProvider services)
            : base(services)
        {
        }

        public override async Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user, IEnumerable<string> scopes)
        {
            var claimsIdentity = await base.CreateIdentityAsync(user, scopes);

            var identity = new ClaimsIdentity(
               "",
               Options.ClaimsIdentity.UserNameClaimType,
               Options.ClaimsIdentity.RoleClaimType);

            var claim = new Claim("given_name", user.GivenName);
            claim.Properties.Add("destination", "id_token token");
            claimsIdentity.AddClaim(claim);

            return claimsIdentity;
        }
    }
}