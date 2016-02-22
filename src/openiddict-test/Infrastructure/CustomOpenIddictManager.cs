using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using openiddicttest.Models;
using OpenIddict.Models;
using System;
using AspNet.Security.OpenIdConnect.Extensions;
using OpenIddict;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace openiddicttest
{
    public class CustomOpenIddictManager : OpenIddict.OpenIddictManager<ApplicationUser, Application>
    {
        public CustomOpenIddictManager(IOpenIddictStore<ApplicationUser, Application> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
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