using AutoMapper;
using DUT.Application.Extensions;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Apps;
using DUT.Constants;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class AppService : IAppService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public AppService(DUTDbContext db, IMapper mapper, IIdentityService identityService)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<AppViewModel>> ChangeAppSecretAsync(int appId)
        {
            var appForUpdate = await _db.Apps.AsNoTracking().SingleOrDefaultAsync(s => s.Id == appId);

            if (appForUpdate == null)
                return Result<AppViewModel>.NotFound("App not found");

            appForUpdate.AppSecret = Generator.CreateAppSecret();
            appForUpdate.PrepareToUpdate(_identityService);

            _db.Apps.Update(appForUpdate);
            await _db.SaveChangesAsync();

            return Result<AppViewModel>.SuccessWithData(_mapper.Map<AppViewModel>(appForUpdate));
        }

        public async Task<Result<AppViewModel>> CreateAppAsync(AppCreateModel app)
        {
            var newApp = _mapper.Map<App>(app);

            newApp.AppId = Generator.CreateAppId();
            newApp.AppSecret = Generator.CreateAppSecret();
            newApp.PrepareToCreate(_identityService);

            await _db.Apps.AddAsync(newApp);
            await _db.SaveChangesAsync();

            return Result<AppViewModel>.SuccessWithData(_mapper.Map<AppViewModel>(newApp));
        }

        public async Task<Result<AppViewModel>> DeleteAppAsync(int id)
        {
            var appForDelete = await _db.Apps.AsNoTracking().SingleOrDefaultAsync(s => s.Id == id);

            if (appForDelete == null)
                return Result<AppViewModel>.NotFound("App not found");

            _db.Apps.Remove(appForDelete);
            await _db.SaveChangesAsync();

            return Result<AppViewModel>.SuccessWithData(_mapper.Map<AppViewModel>(appForDelete));
        }

        public async Task<Result<List<AppViewModel>>> GetAllAppsAsync()
        {
            var allApps = await _db.Apps.AsNoTracking().ToListAsync();

            if (!allApps.Any())
                return Result<List<AppViewModel>>.NotFound("Apps not found");

            var appsToView = _mapper.Map<List<AppViewModel>>(allApps);

            return Result<List<AppViewModel>>.SuccessWithData(appsToView);
        }

        public async Task<Result<AppViewModel>> GetAppByIdAsync(int id)
        {
            var app = await _db.Apps.AsNoTracking().SingleOrDefaultAsync(s => s.Id == id);

            if (app == null)
                return Result<AppViewModel>.NotFound("App by id not found");

            var appToView = _mapper.Map<AppViewModel>(app);

            return Result<AppViewModel>.SuccessWithData(appToView);
        }

        public async Task<Result<AppViewModel>> UpdateAppAsync(AppEditModel app)
        {
            var appToUpdate = await _db.Apps.AsNoTracking().SingleOrDefaultAsync(x => x.Id == app.Id);

            if (appToUpdate == null)
                return Result<AppViewModel>.NotFound("App by id not found");

            appToUpdate.ActiveFrom = app.ActiveFrom;
            appToUpdate.ActiveTo = app.ActiveTo;
            appToUpdate.Description = app.Description;
            appToUpdate.Image = app.Image;
            appToUpdate.IsActive = app.IsActive;
            appToUpdate.Name = app.Name;
            appToUpdate.ShortName = app.ShortName;
            appToUpdate.PrepareToUpdate(_identityService);

            _db.Apps.Update(appToUpdate);
            await _db.SaveChangesAsync();

            return Result<AppViewModel>.SuccessWithData(_mapper.Map<AppViewModel>(appToUpdate));
        }
    }
}
