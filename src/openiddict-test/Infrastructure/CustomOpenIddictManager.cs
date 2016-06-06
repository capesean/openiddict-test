using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict;
using openiddicttest.Models;

namespace openiddicttest
{
    public class CustomOpenIddictManager : OpenIddictTokenManager<OpenIddictToken, ApplicationUser>
    {
        public CustomOpenIddictManager(
            IServiceProvider services,
            IOpenIddictTokenStore<OpenIddictToken> store,
            UserManager<ApplicationUser> users,
            IOptions<IdentityOptions> options,
            ILogger<OpenIddictTokenManager<OpenIddictToken, ApplicationUser>> logger)
            : base(services, store, users, options, logger)
        {
        }

        public override async Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user, IEnumerable<string> scopes)
        {
            var claimsIdentity = await base.CreateIdentityAsync(user, scopes);

            claimsIdentity.AddClaim("given_name", user.GivenName,
                OpenIdConnectConstants.Destinations.AccessToken, 
                OpenIdConnectConstants.Destinations.IdentityToken);

            return claimsIdentity;
        }
    }
}