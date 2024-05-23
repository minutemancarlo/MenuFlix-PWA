using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SharedLibrary;
using System.Data.SqlClient;
using System.Net.Mail;
using Microsoft.AspNetCore.SignalR;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderManagementController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        private readonly IHubContext<NotificationHub> _hubContext;
        public OrderManagementController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, IHubContext<NotificationHub> hubContext)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _httpContextAccessor = httpContextAccessor;
            _env = env;
            _hubContext = hubContext;
        }

        [HttpPost("addorder")]
        public async Task<IActionResult> AddOrder([FromBody] OrderInfo order)
        {
            try
            {
                string imagePath = "add-image.png";
                if (order.PaymentType == 1)
                {
                    imagePath = SaveImageToDisk(order.PaymentImage);
                }
                
                var parameters = new DynamicParameters();
                parameters.Add("@CartId", order.UserId);
                parameters.Add("@OrderType", order.OrderType);
                parameters.Add("@PaymentType", order.PaymentType);
                parameters.Add("@PaymentImage", imagePath);
                parameters.Add("@Pax", order.Pax);
                using (var connection = new SqlConnection(_connectionString))
                {                    
                    await connection.ExecuteAsync(
                        "InsertOrders",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                    await _hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", order);
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

        [HttpGet("getcustomerorderswithcompleted")]
        public async Task<ActionResult<IEnumerable<OrderJson>>> GetOrdersDisplayWithCompleted(string emailaddress)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Email", emailaddress);
                parameters.Add("@Action", 6);
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

        [HttpPost("itemfordineinpickup")]
        public async Task<IActionResult> UpdateItemForDineInPickup([FromBody] string orderId)
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
                        "Update Orders set Status=4 where OrderId=@OrderId;Update OrderHistory set Status=4,UpdatedOn=GETDATE() where OrderId=@OrderId",
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

        [HttpPost("itemforriderupdate")]
        public async Task<IActionResult> UpdateItemForTransit([FromBody] DeliveryStatusUpdate status)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@OrderId", status.OrderId);
                parameters.Add("@Email", status.EmailAddress);
                parameters.Add("@Status", status.Status);

                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    await connection.ExecuteAsync(
                        "Update Orders set Status=@Status where OrderId=@OrderId;" +
                        "Update OrderHistory set Status=@Status,UpdatedOn=GETDATE()," +
                        "UpdatedBy=(Select Top 1 Id from AspNetUsers Where Email=@Email) where OrderId=@OrderId",
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

        [HttpPost("orderforremit")]
        public async Task<IActionResult> OrderForRemit([FromBody] string orderId)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@OrderId", orderId);

                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    await connection.ExecuteAsync(                        
                        "Update Orders set isRemitted=2 where OrderId = @OrderId",
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

        [HttpPost("orderforreceive")]
        public async Task<IActionResult> OrderForReeceive([FromBody] string orderId)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@OrderId", orderId);

                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    await connection.ExecuteAsync(
                        "Update Orders set isRemitted=1 where OrderId = @OrderId",
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

        private string SaveImageToDisk(string base64String)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            string serverAddress = $"{request.Scheme}://{request.Host.Value}";
            string filePath = ""; // Define a variable to hold the file path

            // Convert the base64 string back to byte array
            byte[] bytes = Convert.FromBase64String(base64String);

            // Generate a unique filename
            string fileName = Guid.NewGuid().ToString() + ".jpg"; // You can change the extension based on the image type

            // Combine the file path with the file name
            //filePath = Path.Combine("C:\\MenuFlix", fileName); // Modify the path as needed
            filePath = Path.Combine($"{_env.WebRootPath}\\StoreLogos\\", fileName); // Modify the path as needed

            // Write the byte array to the file
            System.IO.File.WriteAllBytes(filePath, bytes);
            var serverFilePath = $"{serverAddress}/StoreLogos/{fileName}";
            return serverFilePath;
        }

    }
}
