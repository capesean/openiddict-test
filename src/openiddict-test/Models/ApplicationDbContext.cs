using Microsoft.Data.Entity;
using OpenIddict;

namespace openiddicttest.Models
{
    public class ApplicationDbContext : OpenIddictContext<ApplicationUser>
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
