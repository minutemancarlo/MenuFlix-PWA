using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using SharedLibrary;
using System.Net.Mail;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        public UserManagementController(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("getallusers")]
        public async Task<List<UserDataGrid>> GetUsers()
        {
            List<UserDataGrid> users = new List<UserDataGrid>();
            List<UserAdditionalDetails> userAdditionalDetails = new List<UserAdditionalDetails>();
            userAdditionalDetails = await _context.UserAdditionalDetails.ToListAsync();

            if (userAdditionalDetails == null)
            {
                return users;
            }
            else
            {
                foreach (var userDetail in userAdditionalDetails)
                {
                    UserDataGrid userDataGrid = new UserDataGrid();
#pragma warning disable CS8601 // Possible null reference assignment.
                    userDataGrid.UserId = await _context.Users
                                .Where(u => u.Email == userDetail.Email)
                                .Select(u => u.Id)
                                .FirstOrDefaultAsync();
                    userDataGrid.Name = $"{userDetail.FirstName} {userDetail.LastName}";
                    userDataGrid.Email = userDetail.Email;
                    userDataGrid.PhoneNumber= userDetail.PhoneNumber;
#pragma warning restore CS8601 // Possible null reference assignment.

#pragma warning disable CS8601 // Possible null reference assignment.
                    userDataGrid.Role = await _context.UserAdditionalDetails
                                      .Where(a => a.Email == userDetail.Email)
                                      .Join(_context.Users,
                                            a => a.Email,
                                            b => b.Email,
                                            (a, b) => b)
                                      .Join(_context.UserRoles,
                                            b => b.Id,
                                            c => c.UserId,
                                            (b, c) => c)
                                      .Join(_context.Roles,
                                            c => c.RoleId,
                                            d => d.Id,
                                            (c, d) => d.Name)
                                      .FirstOrDefaultAsync();
#pragma warning restore CS8601 // Possible null reference assignment.
                    users.Add(userDataGrid);
                }
            }           
            return users;
        }

    }
}
