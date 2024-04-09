using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SharedLibrary;
using System.Data.SqlClient;
using System.Net.Mail;

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
                parameters.Add("@Action", 0);
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

        [HttpGet("getcustomerordersall")]
        public async Task<ActionResult<IEnumerable<OrderJson>>> GetOrdersDisplayAll(string emailaddress)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Email", emailaddress);
                parameters.Add("@Action", 1);

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

        [HttpGet("getordercount")]
        public async Task<ActionResult<IEnumerable<int>>> GetCartCount(string emailaddress)
        {
            try
            {
                var parameters = new DynamicParameters();
                
                parameters.Add("@Email", emailaddress);
                
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    var count = await connection.QueryAsync<int>("GetOrderCount", parameters, commandType: CommandType.StoredProcedure);
                    return Ok(count);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occured: {ex.Message}");
            }
        }

        [HttpPost("itemready")]
        public async Task<IActionResult> UpdateItemReady([FromBody] List<OrderItemStatus> items)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@Action", 1);
                var itemListTable = new DataTable();
                itemListTable.Columns.Add("OrderId", typeof(string));
                itemListTable.Columns.Add("ItemId", typeof(int));
                itemListTable.Columns.Add("IsReady", typeof(bool));
               
                foreach (var item in items)
                {
                    itemListTable.Rows.Add(
                        item.OrderId,
                         item.ItemId,
                        item.IsReady
                        );
                }
                
                parameters.Add("@OrderItemStatus", itemListTable.AsTableValuedParameter("OrderItemsStatusType"));
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    await connection.ExecuteAsync(
                        "UpdateOrderItemStatus",
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

        [HttpPost("itemaccept")]
        public async Task<IActionResult> UpdateItemAccept([FromBody] string orderId)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@OrderId", orderId);
               
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    await connection.ExecuteAsync(
                        //TODO: Add GETDATE on OrderHistory
                        "Update Orders set Status=2 where OrderId=@OrderId;Update OrderHistory set Status=2,UpdatedOn=GETDATE() where OrderId=@OrderId",
                        parameters,
                        commandType: CommandType.Text);

                    return Ok();
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("itemfordelivery")]
        public async Task<IActionResult> UpdateItemForDelivery([FromBody] string orderId)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@OrderId", orderId);

                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    await connection.ExecuteAsync(
                        //TODO: Add GETDATE on OrderHistory
                        "Update Orders set Status=5 where OrderId=@OrderId;Update OrderHistory set Status=5,UpdatedOn=GETDATE() where OrderId=@OrderId",
                        parameters,
                        commandType: CommandType.Text);

                    return Ok();
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("itemforcancel")]
        public async Task<IActionResult> UpdateItemForCancel([FromBody] string orderId)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@OrderId", orderId);

                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    await connection.ExecuteAsync(
                        //TODO: Add GETDATE on OrderHistory
                        "Update Orders set Status=0 where OrderId=@OrderId;Update OrderHistory set Status=0,UpdatedOn=GETDATE() where OrderId=@OrderId",
                        parameters,
                        commandType: CommandType.Text);

                    return Ok();
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

    }
}
