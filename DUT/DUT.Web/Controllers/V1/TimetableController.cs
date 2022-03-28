using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Timetable;
using DUT.Constants;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class TimetableController : ApiBaseController
    {
        private readonly IPermissionService _permissionService;
        private readonly ITimetableService _timetableService;
        public TimetableController(IPermissionService permissionService, ITimetableService timetableService)
        {
            _permissionService = permissionService;
            _timetableService = timetableService;
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupTimetable(int groupId, DateTime? from, DateTime? to)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Timetable, Permissions.CanView))
                return JsonForbiddenResult();

            if (from == null)
                from = DateTime.Today;
            if (to == null)
                to = DateTime.Today.AddMonths(1);
            if ((from.HasValue && to.HasValue) && from > to)
            {
                from = DateTime.Today;
                to = DateTime.Today.AddMonths(1);
            }
            return JsonResult(await _timetableService.GetTimetableBetweenDatesAsync(groupId, from.Value, to.Value));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTimetable([FromBody] TimetableCreateModel timetable)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Timetable, Permissions.CanCreate))
                return JsonForbiddenResult();

            return JsonResult(await _timetableService.CreateTimetableAsync(timetable));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTimetable([FromBody] TimetableCreateModel timetable)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Timetable, Permissions.CanEdit))
                return JsonForbiddenResult();

            return JsonResult(await _timetableService.UpdateTimetableAsync(timetable));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTimetable(long[] ids)
        {
            if (!_permissionService.HasPermission(PermissionClaims.Timetable, Permissions.CanRemove))
                return JsonForbiddenResult();

            return JsonResult(await _timetableService.RemoveTimetableAsync(ids));
        }
    }
}