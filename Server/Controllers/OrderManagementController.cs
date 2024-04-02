using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SharedLibrary;
using System.Data.SqlClient;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderManagementController : ControllerBase
    {
        private readonly string _connectionString;
        public OrderManagementController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("addorder")]
        public async Task<IActionResult> AddOrder([FromBody] OrderInfo order)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CartId", order.UserId);                
                using (var connection = new SqlConnection(_connectionString))
                {                    
                    await connection.ExecuteAsync(
                        "InsertOrders",
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

        [HttpGet("getcustomerorders")]
        public async Task<ActionResult<IEnumerable<OrderJson>>> GetOrdersDisplay(string emailaddress)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Email", emailaddress);
                using (var connection = new SqlConnection(_connectionString))
                {
                    var ordersDisplay = await connection.QueryAsync<OrderJson>("GetOrders", parameters, commandType: CommandType.StoredProcedure);
                    return Ok(ordersDisplay.ToList());
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occured: {ex.Message}");
            }
        }
    }
}
