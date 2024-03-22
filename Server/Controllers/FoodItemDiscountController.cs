using Dapper;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary;
using System.Data;
using System.Data.SqlClient;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FoodItemDiscountController : ControllerBase
    {
        private readonly string _connectionString;
        public FoodItemDiscountController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("addfooddiscount")]
        public async Task<IActionResult> AddFoodCategory([FromBody] FoodItemDiscount fooditem)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("@FoodItemId", fooditem.FoodItemId);
                parameters.Add("@DiscountName", fooditem.DiscountName);
                parameters.Add("@DiscountAmount", fooditem.DiscountAmount);
                parameters.Add("@IsActive", fooditem.isActive);                
                parameters.Add("@CreatedBy", fooditem.CreatedBy);
                parameters.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    await connection.ExecuteScalarAsync<int>(
                      "InsertFoodItemDiscount",
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
