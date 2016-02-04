using Microsoft.AspNet.Identity;
using openiddicttest.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace openiddicttest
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DatabaseInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task Seed()
        {
            _context.Database.EnsureCreated();

            // Add Mvc.Client to the known applications.
            if (_context.Applications.Any())
            {
                foreach (var application in _context.Applications)
                    _context.Remove(application);
                _context.SaveChanges();
            }

            // no need to register an Application in this example
            //_context.Applications.Add(new Application
            //{
            //    Id = "openiddict-test",
            //    DisplayName = "My test application",
            //    RedirectUri = "http://localhost:58292/signin-oidc",
            //    LogoutRedirectUri = "http://localhost:58292/",
            //    Secret = Crypto.HashPassword("secret_secret_secret"),
            //    Type = OpenIddictConstants.ApplicationTypes.Public
            //});
            //_context.SaveChanges();

            if (_context.Users.Any())
            {
                foreach (var u in _context.Users)
                    _context.Remove(u);
                _context.SaveChanges();
            }

            var email = "user@test.com";
            ApplicationUser user;
            if (await _userManager.FindByEmailAsync(email) == null)
            {
                // use the create rather than addorupdate so can set password
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    GivenName = "Sean"
                };
                await _userManager.CreateAsync(user, "P2ssw0rd!");
            }

            user = await _userManager.FindByEmailAsync(email);
            var roleName = "testrole";
            if (!_context.Roles.Any(r => r.Name.ToLower() == roleName))
            {
                _context.Roles.Add(new ApplicationRole() { Name = roleName });
                _context.SaveChanges();
            }

            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }
    }

    public interface IDatabaseInitializer
    {
        Task Seed();
    }
}
