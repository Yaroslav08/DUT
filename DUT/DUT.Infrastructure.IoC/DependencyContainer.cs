using AutoMapper;
using DUT.Application.Services.Implementations;
using DUT.Application.Services.Interfaces;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Extensions.DeviceDetector;

namespace DUT.Infrastructure.IoC
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddDUTServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region Db
            var connString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<DUTDbContext>(options =>
            {
                options.UseSqlServer(connString);
                options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Error);
            });            
            #endregion

            #region Automapper

            services.AddSingleton(new MapperConfiguration(mc =>
            {
                mc.AddProfile<Application.ViewModels.Mapper>();
            }).CreateMapper());

            #endregion

            #region Services

            services.AddScoped<IUniversityService, UniversityService>();
            services.AddScoped<IIdentityService, HttpIdentityService>();
            services.AddScoped<IInitialService, InitialService>();
            services.AddScoped<IFacultyService, FacultyService>();
            services.AddScoped<ISpecialtyService, SpecialtyService>();
            services.AddScoped<IAppService, AppService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IDiplomaService, DiplomaService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IClaimService, ClaimService>();
            services.AddScoped<ISettingService, SettingService>();

            #endregion

            #region Managers

            services.AddSingleton<ISessionManager, SessionManager>();
            services.AddScoped<IUserManager, UserManager>();

            #endregion

            #region Common

            services.AddHttpContextAccessor();

            #endregion

            #region Libraries

            services.AddDeviceDetector();

            #endregion

            return services;
        }
    }
}