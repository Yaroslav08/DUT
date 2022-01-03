using AutoMapper;
using DUT.Application.Services.Implementations;
using DUT.Application.Services.Interfaces;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddDefaultIdentity<User>(options =>
            {
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);

                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredUniqueChars = 1;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedAccount = false;

                options.Stores.ProtectPersonalData = true;

                options.User.RequireUniqueEmail = true;

            }).AddEntityFrameworkStores<DUTDbContext>();
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

            #endregion


            return services;
        }
    }
}