using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Timetable;
using Microsoft.AspNetCore.Mvc;

namespace DUT.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class TimetableController : ApiBaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ITimetableService _timetableService;
        public TimetableController(IAuthenticationService authenticationService, ITimetableService timetableService)
        {
            _authenticationService = authenticationService;
            _timetableService = timetableService;
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupTimetable(int groupId, DateTime? from, DateTime? to)
        {
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
            return JsonResult(await _timetableService.CreateTimetableAsync(timetable));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTimetable([FromBody] TimetableCreateModel timetable)
        {
            return JsonResult(await _timetableService.UpdateTimetableAsync(timetable));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTimetable(long[] ids)
        {
            return JsonResult(await _timetableService.RemoveTimetableAsync(ids));
        }
    }
}
