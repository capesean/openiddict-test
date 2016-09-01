using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict;
using openiddicttest.Models;
using System.Linq;
using System.Threading.Tasks;

namespace openiddicttest
{
    [Authorize(Roles = "testrole")]
    public class TestController : Controller
    {
        private ApplicationDbContext _context;
        private OpenIddictUserManager<ApplicationUser> _userManager;

        public TestController(ApplicationDbContext context, OpenIddictUserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("api/test"), HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Ok("No user / not logged in");// if Authorize is not applied

            var claims = User.Claims.Select(claim => new { claim.Type, claim.Value }).ToArray();
            return Json(claims);
        }
    }
}