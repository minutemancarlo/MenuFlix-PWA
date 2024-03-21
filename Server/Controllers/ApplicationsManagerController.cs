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
                // Save the file to the server
                string imagePath = SaveImageToDisk(storeApplication.Logo); // Save the image and get the saved path

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
                parameters.Add("@Logo", imagePath); // Save the path to the database
                parameters.Add("@OwnerId", storeApplication.OwnerId); // Assuming OwnerId is the UserId

                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    await connection.ExecuteAsync("InsertStoreApplication", parameters, commandType: CommandType.StoredProcedure);

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

        [HttpGet("getstore")]
        public async Task<ActionResult<StoreApplications>> GetStoreApplication(string emailaddress)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Email", emailaddress);
                parameters.Add("@Type", 0);
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    var userDataGrid = await connection.QueryAsync<StoreApplications>("GetStoreApplication", parameters, commandType: CommandType.StoredProcedure);

                    if (userDataGrid.Any())
                    {
                        // Return the first StoreApplications object from the list
                        return userDataGrid.First();
                    }
                    else
                    {
                        // If no StoreApplications found, return NotFound
                        return NotFound();
                    }
                }
            }
            catch(Exception ex)
            {
                return BadRequest($"Exception Occured: {ex.Message}");
            }
            
        }

        [HttpGet("getstoreall")]
        public async Task<ActionResult<IEnumerable<StoreApplications>>> GetStoreApplicationAll()
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Email", String.Empty);
                parameters.Add("@Type", 1);
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    var userDataGrid = await connection.QueryAsync<StoreApplications>("GetStoreApplication", parameters, commandType: CommandType.StoredProcedure);
                    return Ok(userDataGrid.ToList());
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occured: {ex.Message}");
            }
        }


        private string SaveImageToDisk(string base64String)
        {
            string filePath = ""; // Define a variable to hold the file path

            // Convert the base64 string back to byte array
            byte[] bytes = Convert.FromBase64String(base64String);

            // Generate a unique filename
            string fileName = Guid.NewGuid().ToString() + ".jpg"; // You can change the extension based on the image type

            // Combine the file path with the file name
            filePath = Path.Combine("C:\\MenuFlix", fileName); // Modify the path as needed

            // Write the byte array to the file
            System.IO.File.WriteAllBytes(filePath, bytes);
            
            return filePath; // Return the saved file path
        }

    }
}
