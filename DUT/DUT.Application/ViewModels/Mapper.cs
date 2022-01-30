using AutoMapper;
using DUT.Application.ViewModels.Apps;
using DUT.Application.ViewModels.Faculty;
using DUT.Application.ViewModels.Group;
using DUT.Application.ViewModels.Group.GroupMember;
using DUT.Application.ViewModels.Notification;
using DUT.Application.ViewModels.Session;
using DUT.Application.ViewModels.Specialty;
using DUT.Application.ViewModels.University;
using DUT.Application.ViewModels.User;
using System.Text;

namespace DUT.Application.ViewModels
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Domain.Models.University, UniversityViewModel>();
            CreateMap<Domain.Models.University, UniversityCreateModel>().ReverseMap();
            CreateMap<Domain.Models.University, UniversityEditModel>().ReverseMap();

            CreateMap<Domain.Models.Faculty, FacultyViewModel>();
            CreateMap<Domain.Models.Faculty, FacultyCreateModel>().ReverseMap();
            CreateMap<Domain.Models.Faculty, FacultyEditModel>().ReverseMap();

            CreateMap<Domain.Models.Specialty, SpecialtyViewModel>();
            CreateMap<Domain.Models.Specialty, SpecialtyCreateModel>().ReverseMap();
            CreateMap<Domain.Models.Specialty, SpecialtyEditModel>().ReverseMap();


            CreateMap<Domain.Models.Group, GroupViewModel>()
                .ForMember(x => x.Name, s => s.MapFrom(x => $"{x.Name} ({x.StartStudy.Year})"));

            CreateMap<Domain.Models.GroupInvite, GroupInviteViewModel>();

            CreateMap<Domain.Models.UserGroupRole, UserGroupRoleViewModel>().ReverseMap();


            CreateMap<Domain.Models.User, UserViewModel>()
                .ForMember(x => x.FullName, s => s.MapFrom(s => BuildFullName(s)));
            CreateMap<Domain.Models.User, UserShortViewModel>().ReverseMap();

            CreateMap<Domain.Models.App, AppViewModel>().ReverseMap();
            CreateMap<Domain.Models.App, AppEditModel>().ReverseMap();
            CreateMap<Domain.Models.App, AppCreateModel>().ReverseMap();

            CreateMap<Domain.Models.Session, SessionViewModel>().ReverseMap();

            CreateMap<Domain.Models.Notification, NotificationViewModel>().ReverseMap();
        }

        private string BuildFullName(Domain.Models.User user)
        {
            var sb = new StringBuilder();
            sb.Append(user.LastName);
            sb.Append(" ");
            sb.Append(user.FirstName);
            if (!string.IsNullOrEmpty(user.MiddleName) && !string.IsNullOrWhiteSpace(user.MiddleName))
            {
                sb.Append(" ");
                sb.Append(user.MiddleName);
            }
            return sb.ToString();
        }
    }
}