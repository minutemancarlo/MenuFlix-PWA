using Dapper;
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

    public class FoodCategoryController : ControllerBase
    {
        private readonly string _connectionString;
        public FoodCategoryController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("addfoodcategory")]
        public async Task<IActionResult> AddFoodCategory([FromBody] CategoryWithEmail category)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CategoryName", category.Category.Name);
                parameters.Add("@CategoryDescription", category.Category.Description);
                parameters.Add("@Email", category.Email);
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    await connection.ExecuteAsync(
                        "InsertCategory",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return Ok();
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("getcategories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories(string email)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Email", email);                
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    var categories = await connection.QueryAsync<Category>("GetCategories", parameters, commandType: CommandType.StoredProcedure);
                    return Ok(categories.ToList());
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occured: {ex.Message}");
            }
        }

    }
}
