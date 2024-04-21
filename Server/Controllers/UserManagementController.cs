using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using SharedLibrary;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly string _connectionString;
        public UserManagementController(UserManager<AppUser> userManager, AppDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("getallusers")]
        public async Task<List<UserDataGrid>> GetUsers()
        {
            List<UserDataGrid> users = new List<UserDataGrid>();
            List<UserAdditionalDetails> userAdditionalDetails = new List<UserAdditionalDetails>();
            userAdditionalDetails = await _context.UserAdditionalDetails.ToListAsync();

            if (userAdditionalDetails == null)
            {
                return users;
            }
            else
            {
                foreach (var userDetail in userAdditionalDetails)
                {
                    UserDataGrid userDataGrid = new UserDataGrid();
#pragma warning disable CS8601 // Possible null reference assignment.
                    userDataGrid.UserId = await _context.Users
                                .Where(u => u.Email == userDetail.Email)
                                .Select(u => u.Id)
                                .FirstOrDefaultAsync();
                    userDataGrid.Name = $"{userDetail.FirstName} {userDetail.LastName}";
                    userDataGrid.Email = userDetail.Email;
                    userDataGrid.PhoneNumber = userDetail.PhoneNumber;
#pragma warning restore CS8601 // Possible null reference assignment.

#pragma warning disable CS8601 // Possible null reference assignment.
                    userDataGrid.Role = await _context.UserAdditionalDetails
                                      .Where(a => a.Email == userDetail.Email)
                                      .Join(_context.Users,
                                            a => a.Email,
                                            b => b.Email,
                                            (a, b) => b)
                                      .Join(_context.UserRoles,
                                            b => b.Id,
                                            c => c.UserId,
                                            (b, c) => c)
                                      .Join(_context.Roles,
                                            c => c.RoleId,
                                            d => d.Id,
                                            (c, d) => d.Name)
                                      .FirstOrDefaultAsync();
#pragma warning restore CS8601 // Possible null reference assignment.
                    users.Add(userDataGrid);
                }
            }
            return users;
        }

        [HttpGet("getuserdetails")]
        public async Task<UserDataGrid> GetUserDetails(string email)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email);

            using (var connection = new SqlConnection(_connectionString))
            {
                // Execute the stored procedure
                var userDataGrid = await connection.QueryAsync<UserDataGrid>("GetUserDetails", parameters, commandType: CommandType.StoredProcedure);

                return (UserDataGrid)userDataGrid;
            }
        }

        [HttpGet("getuserid")]
        public async Task<ActionResult<string>> GetUserId(string email)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Email", email);

                using (var connection = new SqlConnection(_connectionString))
                {
                    var userid = await connection.QueryFirstOrDefaultAsync<string>(
                        "SELECT Id AS UserId FROM AspNetUsers WHERE Email = @Email",
                        parameters);

                    if (userid != null)
                    {
                        return Ok(userid);
                    }
                    else
                    {
                        return NotFound(); // Assuming you want to return 404 if the user is not found
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occurred: {ex.Message}");
            }
        }

        [HttpGet("getuserslist")]
        public async Task<ActionResult<IEnumerable<UsersDisplay>>> GetUsersList()
        {
            try
            {
                var parameters = new DynamicParameters();                
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    var userDataGrid = await connection.QueryAsync<UsersDisplay>("GetUsersList", parameters, commandType: CommandType.StoredProcedure);
                    return Ok(userDataGrid.ToList());
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occured: {ex.Message}");
            }
        }

    }
}
