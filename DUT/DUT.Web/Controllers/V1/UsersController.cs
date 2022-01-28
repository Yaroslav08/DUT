using DUT.Application.Options;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.User;
using Microsoft.AspNetCore.Mvc;
namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class UsersController : ApiBaseController
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLastUsers()
        {
            return JsonResult(await _userService.GetLastUsersAsync(5));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            return JsonResult(await _userService.GetUserByIdAsync(id));
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchUsers([FromBody] SearchUserOptions searchUserOptions)
        {
            return JsonResult(await _userService.SearchUsersAsync(searchUserOptions));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateModel model)
        {
            return JsonResult(await _userService.CreateUserAsync(model));
        }
    }
}