using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using SharedLibrary;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class UserAdditionalDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _connectionString;

        public UserAdditionalDetailsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public async Task<ActionResult<UserAdditionalDetails>> GetUserAdditionalDetails(string emailaddress)
        {
            var userAdditionalDetails = await _context.UserAdditionalDetails.FirstOrDefaultAsync(u => u.Email == emailaddress);


            if (userAdditionalDetails == null)
            {
                return NotFound();
            }

            return userAdditionalDetails;
        }

        [HttpPost("createupdate")]
        public async Task<IActionResult> CreateUserAdditionalDetails([FromBody] UserAdditionalDetails user)
        {

            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Guid.NewGuid());
                parameters.Add("@FirstName", user.FirstName);
                parameters.Add("@LastName", user.LastName);
                parameters.Add("@Email", user.Email);
                parameters.Add("@PhoneNumber", user.PhoneNumber);
                parameters.Add("@AddressLine1", user.AddressLine1);
                parameters.Add("@AddressLine2", user.AddressLine2);
                parameters.Add("@CityTown", user.CityTown);
                parameters.Add("@Province", user.Province);
                parameters.Add("@PostalCode", user.PostalCode);          
                parameters.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    await connection.ExecuteScalarAsync<int>(
                      "InsertUserAdditionalDetails",
                      parameters,
                      commandType: CommandType.StoredProcedure);
                    var statusCode = parameters.Get<int>("@StatusCode");
                    if (statusCode == 1)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest($"An error occurred: {statusCode}");
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }            
        }
    }
}
