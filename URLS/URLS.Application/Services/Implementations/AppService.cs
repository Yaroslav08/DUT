using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Apps;
using URLS.Constants;
using URLS.Constants.APIResponse;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class AppService : IAppService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public AppService(URLSDbContext db, IMapper mapper, IIdentityService identityService)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<AppViewModel>> ChangeAppSecretAsync(int appId)
        {
            var appForUpdate = await _db.Apps.AsNoTracking().FirstOrDefaultAsync(s => s.Id == appId);

            if (appForUpdate == null)
                return Result<AppViewModel>.NotFound(typeof(App).NotFoundMessage(appId));

            if (!_identityService.IsAdministrator())
                if (appForUpdate.UserId != _identityService.GetUserId())
                    return Result<AppViewModel>.Forbiden();

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
            newApp.UserId = _identityService.GetUserId();
            newApp.PrepareToCreate(_identityService);

            await _db.Apps.AddAsync(newApp);
            await _db.SaveChangesAsync();

            return Result<AppViewModel>.Created(_mapper.Map<AppViewModel>(newApp));
        }

        public async Task<Result<bool>> DeleteAppAsync(int id)
        {
            var appForDelete = await _db.Apps.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);

            if (appForDelete == null)
                return Result<bool>.NotFound(typeof(App).NotFoundMessage(id));

            if (!_identityService.IsAdministrator())
                if (appForDelete.UserId != _identityService.GetUserId())
                    return Result<bool>.Forbiden();

            _db.Apps.Remove(appForDelete);
            await _db.SaveChangesAsync();

            return Result<bool>.Success();
        }

        public async Task<Result<List<AppViewModel>>> GetAllAppsAsync(int offset = 0, int limit = 20)
        {
            int count = 0;
            var query = _db.Apps.AsNoTracking();

            if (!_identityService.IsAdministrator())
            {
                query = query.Where(s => s.UserId == _identityService.GetUserId());
                count = await _db.Apps.CountAsync(s => s.UserId == _identityService.GetUserId());
            }
            else
                count = await _db.Apps.CountAsync();

            query = query.OrderBy(s => s.Id).Skip(offset).Take(limit);
            var allApps = await query.ToListAsync();

            var appsToView = _mapper.Map<List<AppViewModel>>(allApps);

            return Result<List<AppViewModel>>.SuccessList(appsToView, Meta.FromMeta(count, offset, limit));
        }

        public async Task<Result<AppViewModel>> GetAppByIdAsync(int id)
        {
            var app = await _db.Apps.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);

            if (app == null)
                return Result<AppViewModel>.NotFound(typeof(App).NotFoundMessage(id));

            if (!_identityService.IsAdministrator())
                if (app.UserId != _identityService.GetUserId())
                    return Result<AppViewModel>.Forbiden();

            var appToView = _mapper.Map<AppViewModel>(app);

            return Result<AppViewModel>.SuccessWithData(appToView);
        }

        public async Task<Result<App>> GetAppBySchemeAsync(string scheme)
        {
            var app = await _db.Apps.AsNoTracking().FirstOrDefaultAsync(s => s.Scheme == scheme);
            if (app == null)
                return Result<App>.NotFound(typeof(App).NotFoundMessage(scheme));
            return Result<App>.SuccessWithData(app);
        }

        public async Task<Result<AppDetail>> GetAppDetailsAsync(int id)
        {
            var app = await _db.Apps.AsNoTracking().FirstOrDefaultAsync(app => app.Id == id);
            if (app == null)
                return Result<AppDetail>.NotFound(typeof(App).NotFoundMessage(id));

            if (!_identityService.IsAdministrator())
                if (app.UserId != _identityService.GetUserId())
                    return Result<AppDetail>.Forbiden();

            return Result<AppDetail>.SuccessWithData(new AppDetail
            {
                AppId = app.AppId,
                AppSecret = app.AppSecret,
            });
        }

        public async Task<Result<AppViewModel>> UpdateAppAsync(AppEditModel app)
        {
            var appToUpdate = await _db.Apps.AsNoTracking().FirstOrDefaultAsync(x => x.Id == app.Id);

            if (appToUpdate == null)
                return Result<AppViewModel>.NotFound(typeof(App).NotFoundMessage(app.Id));

            if (!_identityService.IsAdministrator())
                if (appToUpdate.UserId != _identityService.GetUserId())
                    return Result<AppViewModel>.Forbiden();

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