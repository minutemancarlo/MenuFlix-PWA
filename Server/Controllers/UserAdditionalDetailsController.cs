using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using SharedLibrary;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class UserAdditionalDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserAdditionalDetailsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<UserAdditionalDetails>> GetUserAdditionalDetails(string emailaddress)
        {
            var userAdditionalDetails = await _context.UserAdditionalDetails.FirstOrDefaultAsync(u => u.Email == emailaddress);


            if (userAdditionalDetails == null)
            {
                return NotFound();
            }

            return userAdditionalDetails;
        }

        [HttpPost("createupdate")]
        public async Task<IActionResult> CreateUserAdditionalDetails([FromBody] UserAdditionalDetails userAdditionalDetails)
        {
            var existingUserAdditionalDetails = await _context.UserAdditionalDetails.FirstOrDefaultAsync(u => u.Email == userAdditionalDetails.Email);

            if (existingUserAdditionalDetails == null)
            {
                userAdditionalDetails.Id = Guid.NewGuid();
                _context.UserAdditionalDetails.Add(userAdditionalDetails);
            }
            else
            {                
                _context.Attach(existingUserAdditionalDetails);
                existingUserAdditionalDetails.FirstName = userAdditionalDetails.FirstName;
                existingUserAdditionalDetails.LastName = userAdditionalDetails.LastName;
                existingUserAdditionalDetails.AddressLine1 = userAdditionalDetails.AddressLine1;
                existingUserAdditionalDetails.AddressLine2 = userAdditionalDetails.AddressLine2;
                existingUserAdditionalDetails.CityTown = userAdditionalDetails.CityTown;
                existingUserAdditionalDetails.Province = userAdditionalDetails.Province;
                existingUserAdditionalDetails.PostalCode = userAdditionalDetails.PostalCode;
                existingUserAdditionalDetails.PhoneNumber = userAdditionalDetails.PhoneNumber;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok();
        }
    }
}
