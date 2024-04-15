using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SharedLibrary;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DashboardManagerController : ControllerBase
    {
        private readonly string _connectionString;

        public DashboardManagerController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        

        [HttpGet("getcashierdashboardall")]
        public async Task<ActionResult<IEnumerable<CashierDashboardDisplay>>> GetCashierDashboard1(string emailaddress)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Chart", 0);
                parameters.Add("@Email", emailaddress);
                using (var connection = new SqlConnection(_connectionString))
                {
                    var results = await connection.QueryAsync<CashierDashboardDisplay>("GetCashierChart", parameters, commandType: CommandType.StoredProcedure);
                    return Ok(results.ToList());
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occured: {ex.Message}");
            }
        }

        [HttpGet("getcashierdashboardall2")]
        public async Task<ActionResult<IEnumerable<CashierDashboardDisplay>>> GetCashierDashboard2(string emailaddress)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Chart", 1);
                parameters.Add("@Email", emailaddress);
                using (var connection = new SqlConnection(_connectionString))
                {
                    var results = await connection.QueryAsync<CashierDashboardDisplay>("GetCashierChart", parameters, commandType: CommandType.StoredProcedure);
                    return Ok(results.ToList());
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception Occured: {ex.Message}");
            }
        }
    }
}
