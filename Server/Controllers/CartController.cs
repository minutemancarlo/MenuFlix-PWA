using Dapper;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary;
using System.Data;
using System.Data.SqlClient;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {

        private readonly string _connectionString;
        public CartController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("cartactions")]
        public async Task<IActionResult> AddToCart([FromBody] Cart cart)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Action", cart.Actions);
                parameters.Add("@CartId", cart.CartId);
                parameters.Add("@ItemId", cart.ItemId);
                parameters.Add("@Pcs", cart.Pcs);
                parameters.Add("@UserId", cart.UserId);
                parameters.Add("@IsProcessed", cart.IsProcessed);               
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    await connection.ExecuteAsync(
                        "CartActions",
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

        [HttpGet("getcartcount")]
        public async Task<ActionResult<IEnumerable<int>>> GetCartCount(string emailaddress)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Action", 4);
                parameters.Add("@CartId", 0);
                parameters.Add("@ItemId", 0);
                parameters.Add("@Pcs", 0);
                parameters.Add("@UserId", emailaddress);
                parameters.Add("@IsProcessed", 0);
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    var count = await connection.QueryAsync<int>("CartActions", parameters, commandType: CommandType.StoredProcedure);
                    return Ok(count);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occured: {ex.Message}");
            }
        }
    }
}
