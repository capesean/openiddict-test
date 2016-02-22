using Microsoft.EntityFrameworkCore;
using OpenIddict;
using OpenIddict.Models;

namespace openiddicttest.Models
{
    public class ApplicationDbContext : OpenIddictContext<ApplicationUser, Application, ApplicationRole, string>
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
