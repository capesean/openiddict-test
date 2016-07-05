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
	public class CustomOpenIddictManager : OpenIddictUserManager<ApplicationUser>
	{
		public CustomOpenIddictManager(
			IServiceProvider services,
			IOpenIddictUserStore<ApplicationUser> store,
			IOptions<IdentityOptions> options,
			ILogger<OpenIddictUserManager<ApplicationUser>> logger,
			IPasswordHasher<ApplicationUser> hasher,
			IEnumerable<IUserValidator<ApplicationUser>> userValidators,
			IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
			ILookupNormalizer keyNormalizer,
			IdentityErrorDescriber errors
			)
			: base(services, store, options, logger, hasher, userValidators, passwordValidators, keyNormalizer, errors)
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