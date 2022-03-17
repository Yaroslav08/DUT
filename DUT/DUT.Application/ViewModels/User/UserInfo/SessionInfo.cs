using DUT.Application.ViewModels.Session;

namespace DUT.Application.ViewModels.User.UserInfo
{
    public class SessionInfo
    {
        public List<SessionViewModel> Sessions { get; set; }
        public int TotalSessions { get; set; }
        public int ActiveSessions { get; set; }
    }
}