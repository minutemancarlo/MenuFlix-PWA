using Dapper;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary;
using System.Data;
using System.Data.SqlClient;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FoodItemController : ControllerBase
    {
        private readonly string _connectionString;
        public FoodItemController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("addfooditem")]
        public async Task<IActionResult> AddFoodCategory([FromBody] FoodItem fooditem)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("@ItemName", fooditem.Name);
                parameters.Add("@ItemDescription", fooditem.Description);
                parameters.Add("@ItemImage", fooditem.Logo);
                parameters.Add("@ItemPrice", fooditem.Price);
                parameters.Add("@ItemCategory", fooditem.CategoryName);
                parameters.Add("@Email", fooditem.Email);
                parameters.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                      await connection.ExecuteScalarAsync<int>(
                        "InsertFoodItem",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                    var statusCode = parameters.Get<int>("@StatusCode");
                    if (statusCode == 1)
                    {
                        return Ok();
                    }
                    else if (statusCode == 0)
                    {
                        return Conflict("Item already exists.");
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

        [HttpGet("getfooditems")]
        public async Task<ActionResult<IEnumerable<FoodItemDataGrid>>> GetFoodItems(string email)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Email", email);
                using (var connection = new SqlConnection(_connectionString))
                {                    
                    var fooditems = await connection.QueryAsync<FoodItemDataGrid>("GetFoodItems", parameters, commandType: CommandType.StoredProcedure);
                    return Ok(fooditems.ToList());
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occured: {ex.Message}");
            }
        }

        [HttpPost("changeitemstatus")]
        public async Task<IActionResult> ChangeItemStatus([FromBody] FoodItemStatus status)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("@ItemId", status.ItemId);
                parameters.Add("@IsDisabled", status.IsDisabled);
                parameters.Add("@Email", status.Email);
                parameters.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    await connection.ExecuteScalarAsync<int>(
                      "UpdateFoodItemStatus",
                      parameters,
                      commandType: CommandType.StoredProcedure);
                    var statusCode = parameters.Get<int>("@StatusCode");
                    if (statusCode == 1)
                    {
                        return Ok();
                    }
                    else if (statusCode == 0)
                    {
                        return Conflict("Item already exists.");
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
