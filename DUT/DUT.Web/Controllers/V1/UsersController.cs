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
        private readonly IIdentityService _identityService;
        public UsersController(IUserService userService, IIdentityService identityService)
        {
            _userService = userService;
            _identityService = identityService;
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

        [HttpPatch("username")]
        public async Task<IActionResult> UpdateUsername([FromBody] UsernameUpdateModel model)
        {
            if (model.UserId == null)
                model.UserId = _identityService.GetUserId();
            return JsonResult(await _userService.UpdateUsernameAsync(model));
        }
    }
}