using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using openiddicttest.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace openiddict_test.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Register(ApplicationUser user, string password, IEnumerable<ApplicationRole> roles)
        {
            if (await _userManager.FindByEmailAsync(user.Email) != null)
                return new BadRequestObjectResult($"User {user.Email} already exists.");

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return new ObjectResult($"User {user.Email} already exists.") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //todo: send email
            return new OkObjectResult($"User {user.Email} created, confirmation email sent.");
        }

        public async Task<IActionResult> Confirm(string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new BadRequestObjectResult($"Invalid email: {email}");

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
                return new BadRequestObjectResult($"Bad confirmation code for email: {email}");

            return new OkObjectResult($"User {email} confirmed");
        }
    }
}