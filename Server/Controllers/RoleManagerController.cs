using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using SharedLibrary;
using Microsoft.EntityFrameworkCore;
using Client.Components.Pages.Administrator;
using System.Data.SqlClient;
using Dapper;
using System.Data;
using System.Reflection.Metadata;
namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleManagerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly string _connectionString;
        public RoleManagerController(UserManager<AppUser> userManager, AppDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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

        [HttpPost("updateuserrole")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UserDataGrid userDataGrid)
        {
            // Set up your parameters
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userDataGrid.UserId);
            parameters.Add("@Role", userDataGrid.Role);

            using (var connection = new SqlConnection(_connectionString))
            {
                // Execute the stored procedure
                await connection.ExecuteAsync(
                    "UpdateUserRole",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                
                return Ok();
            }
        }



    }
}
