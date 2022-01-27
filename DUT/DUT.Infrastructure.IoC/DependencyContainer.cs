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
            services.AddScoped<IIdentityService, FakeIdentityService>();
            services.AddScoped<IInitialService, InitialService>();
            services.AddScoped<IFacultyService, FacultyService>();
            services.AddScoped<ISpecialtyService, SpecialtyService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAppService, AppService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IUserService, UserService>();

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