using DUT.Application.Services.Interfaces;
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



    }
}
