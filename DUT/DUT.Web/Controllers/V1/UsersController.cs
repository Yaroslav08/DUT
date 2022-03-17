using DUT.Application.Options;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.User;
using DUT.Constants;
using Microsoft.AspNetCore.Mvc;
namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class UsersController : ApiBaseController
    {
        private readonly IUserService _userService;
        private readonly IIdentityService _identityService;
        private readonly IPermissionService _permissionService;
        public UsersController(IUserService userService, IIdentityService identityService, IPermissionService permissionService)
        {
            _userService = userService;
            _identityService = identityService;
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLastUsers()
        {
            if (!_permissionService.HasPermission(PermissionClaims.Users, Permissions.CanViewAll))
                return JsonForbiddenResult();
            return JsonResult(await _userService.GetLastUsersAsync(5));
        }

        [HttpGet("teachers")]
        public async Task<IActionResult> GetTeachers(int offset = 0, int count = 20)
        {
            return JsonResult(await _userService.GetTeachersAsync(offset, count));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Users, Permissions.CanView))
                return JsonForbiddenResult();
            if (id == _identityService.GetUserId())
                return await GetMe();
            return JsonResult(await _userService.GetUserByIdAsync(id));
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchUsers([FromBody] SearchUserOptions searchUserOptions)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Users, Permissions.Search))
                return JsonForbiddenResult();
            return JsonResult(await _userService.SearchUsersAsync(searchUserOptions));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateModel model)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Users, Permissions.CanCreate))
                return JsonForbiddenResult();
            return JsonResult(await _userService.CreateUserAsync(model));
        }

        [HttpPatch("username")]
        public async Task<IActionResult> UpdateUsername([FromBody] UsernameUpdateModel model)
        {
            //todo create new permission
            if (model.UserId == null)
                model.UserId = _identityService.GetUserId();
            return JsonResult(await _userService.UpdateUsernameAsync(model));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var user = await _userService.GetFullInfoUserByIdAsync(_identityService.GetUserId());
            return JsonResult(user);
        }
    }
}