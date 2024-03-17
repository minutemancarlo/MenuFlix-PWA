using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Data;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        public UserManagementController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        //private async Task GetIfUserEmpty()
        //{
        //    _userManager.
        //}

    }
}
