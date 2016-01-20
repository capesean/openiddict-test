using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using openiddicttest.Models;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize]
public class TestController : Controller
{
    private ApplicationDbContext _context;
    private UserManager<ApplicationUser> _userManager;

    public TestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Route("api/test"), HttpGet]
    public async Task<IActionResult> Get()
    {
        var user = await _userManager.FindByIdAsync(User.GetUserId());
        if (user == null) return Ok("No user / not logged in");// if Authorize is not applied
        return Ok(user);
    }
}