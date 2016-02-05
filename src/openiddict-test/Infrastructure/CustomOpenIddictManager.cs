using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using OpenIddict.Models;
using openiddicttest.Models;

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
            var identity = await base.CreateIdentityAsync(user, scopes);

            var name = new Claim("given_name", user.GivenName)
                .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken,
                                 OpenIdConnectConstants.Destinations.IdentityToken);

            identity.AddClaim(name);

            return identity;
        }
    }
}