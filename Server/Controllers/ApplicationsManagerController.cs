using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using SharedLibrary;
using System.Data;
using System.Data.SqlClient;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]    
    public class ApplicationsManagerController : ControllerBase
    {
        private readonly string _connectionString;
        public ApplicationsManagerController(IConfiguration configuration)
        {            
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("registerstore")]
        public async Task<IActionResult> RegisterStore([FromBody] StoreApplications storeApplication)
        {
            try
            {
                // Create parameters for the stored procedure
                var parameters = new DynamicParameters();
                parameters.Add("@Name", storeApplication.Name);
                parameters.Add("@Email", storeApplication.Email);
                parameters.Add("@PhoneNumber", storeApplication.PhoneNumber);
                parameters.Add("@TIN", storeApplication.TIN);
                parameters.Add("@AddressLine1", storeApplication.AddressLine1);
                parameters.Add("@AddressLine2", storeApplication.AddressLine2);
                parameters.Add("@CityTown", storeApplication.CityTown);
                parameters.Add("@Province", storeApplication.Province);
                parameters.Add("@PostalCode", storeApplication.PostalCode);
                parameters.Add("@Logo", storeApplication.Logo);
                parameters.Add("@OwnerId", storeApplication.OwnerId); // Assuming OwnerId is the UserId

                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    await connection.ExecuteAsync("InsertStoreApplication", parameters,commandType: CommandType.StoredProcedure);

                    return Ok("Store registered successfully!");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you can replace Console.WriteLine with your preferred logging mechanism)
                Console.WriteLine($"An error occurred while registering the store: {ex.Message}");

                // Return BadRequest with error message
                return BadRequest($"An error occurred while registering the store: {ex.Message}");
            }
        }


    }
}
