using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using SharedLibrary;
using Microsoft.EntityFrameworkCore;
namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleManagerController : ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
        public RoleManagerController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("adduserrole")]        
        public async Task<IActionResult> AddUserRole([FromBody] RoleAssign roleAssign)
        {
            var userManager = _userManager; 

            int usercount = await userManager.Users.CountAsync();
            roleAssign.Role = usercount > 1 ? "Customer" : "Administrator";
            var user = await userManager.FindByNameAsync(roleAssign.Email);
            Console.WriteLine($"Users: {user}");
            if (user == null)
            {
                return NotFound(); // Handle user not found
            }

            var result = await userManager.AddToRoleAsync(user, roleAssign.Role);
            Console.WriteLine($"Result: {result}");
            if (!result.Succeeded)
            {
                // Handle adding role failure (e.g., display error messages)
                //return BadRequest(result.Errors);
                return BadRequest($"Users: {user} Result: {result}");
            }

            return Ok("Role assigned successfully"); // Or redirect to appropriate page
        }
    }
}
