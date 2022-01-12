using AutoMapper;
using DUT.Application.Services.Implementations;
using DUT.Application.Services.Interfaces;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using DUT.Infrastructure.IoC.Identity;
using Microsoft.AspNetCore.Identity;
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
            //services.AddScoped<IInitialService, InitialService>();
            services.AddScoped<IFacultyService, FacultyService>();
            services.AddScoped<ISpecialtyService, SpecialtyService>();

            #endregion

            #region Managers

            services.AddSingleton<ISessionManager, SessionManager>();

            #endregion

            #region Common

            services.AddHttpContextAccessor();

            #endregion

            return services;
        }
    }
}