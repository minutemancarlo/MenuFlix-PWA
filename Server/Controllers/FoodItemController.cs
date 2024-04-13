using Client.Components.Pages.Customer;
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
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FoodItemController(IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("addfooditem")]
        public async Task<IActionResult> AddFoodCategory([FromBody] FoodItem fooditem)
        {
            try
            {
                // Save the file to the server
                string imagePath = SaveImageToDisk(fooditem.Logo); // Save the image and get the saved path

                var parameters = new DynamicParameters();
                parameters.Add("@ItemName", fooditem.Name);
                parameters.Add("@ItemDescription", fooditem.Description);
                parameters.Add("@ItemImage", imagePath);
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

        [HttpPost("updateratings")]
        public async Task<IActionResult> UpdateRatings([FromBody] UpdateRatings item)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@OrderId", item.OrderId);
                parameters.Add("@UserId", item.UserId);
                parameters.Add("@Rate", item.Rate);
                parameters.Add("@ItemId", item.ItemId);

                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                    await connection.ExecuteAsync(
                        //TODO: Add GETDATE on OrderHistory
                        "Insert into Ratings (ItemId,Rate,UserId,OrderId) values (@ItemId,@Rate,@UserId,@OrderId);",
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


        [HttpPost("updatefooditem")]
        public async Task<IActionResult> UpdateFoodItem([FromBody] FoodItem fooditem)
        {
            try
            {
                string imagePath = "0";
                // Save the file to the server
                if (fooditem.Logo != "0")
                {
                    imagePath = SaveImageToDisk(fooditem.Logo); // Save the image and get the saved path
                }                
                

                var parameters = new DynamicParameters();
                parameters.Add("@ItemId", fooditem.ItemId);
                parameters.Add("@ItemName", fooditem.Name);
                parameters.Add("@ItemDescription", fooditem.Description);
                parameters.Add("@ItemImage", imagePath);
                parameters.Add("@ItemPrice", fooditem.Price);
                parameters.Add("@ItemCategory", fooditem.CategoryName);
                parameters.Add("@UpdatedBy", fooditem.UpdatedBy);
                parameters.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Execute the stored procedure
                      await connection.ExecuteScalarAsync<int>(
                        "UpdateFoodItem",
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

        [HttpGet("getfooditemsrandom")]
        public async Task<ActionResult<IEnumerable<CarouselDisplay>>> GetFoodItemsRandom()
        {
            try
            {
                
                using (var connection = new SqlConnection(_connectionString))
                {
                    var fooditems = await connection.QueryAsync<CarouselDisplay>("GetFoodItemsCustomer", commandType: CommandType.StoredProcedure);
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
            filePath = Path.Combine($"{_env.WebRootPath}\\Fooditems\\", fileName); // Modify the path as needed

            // Write the byte array to the file
            System.IO.File.WriteAllBytes(filePath, bytes);
            var serverFilePath = $"{serverAddress}/Fooditems/{fileName}";
            return serverFilePath;
        }


    }
}
