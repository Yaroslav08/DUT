using AutoMapper;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Setting;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace URLS.Application.Services.Implementations
{
    public class SettingService : BaseService<Setting>, ISettingService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public SettingService(URLSDbContext db, IMapper mapper, IIdentityService identityService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<SettingViewModel>> CreateSettingAsync(SettingCreateModel model)
        {
            if (await CountAsync() >= 1)
                return Result<SettingViewModel>.Error("Can't create more then 1 item");

            var newSetting = new Setting
            {
                FirtsSemesterStart = model.FirtsSemesterStart,
                FirtsSemesterEnd = model.FirtsSemesterEnd,
                SecondSemesterStart = model.SecondSemesterStart,
                SecondSemesterEnd = model.SecondSemesterEnd,
                MaxCourseInUniversity = model.MaxCourseInUniversity,
            };
            newSetting.PrepareToCreate(_identityService);

            await _db.Settings.AddAsync(newSetting);
            await _db.SaveChangesAsync();

            return Result<SettingViewModel>.SuccessWithData(_mapper.Map<SettingViewModel>(newSetting));
        }

        public async Task<Result<Setting>> GetRootSettingAsync()
        {
            var setting = await _db.Settings.AsNoTracking().FirstOrDefaultAsync();
            if (setting == null)
                return Result<Setting>.NotFound("Setting not found");
            return Result<Setting>.SuccessWithData(setting);
        }

        public async Task<Result<SettingViewModel>> UpdateSettingAsync(SettingEditModel model)
        {
            var settingToUpdate = await _db.Settings.AsNoTracking().FirstOrDefaultAsync(s => s.Id == model.Id);
            if (settingToUpdate == null)
                return Result<SettingViewModel>.NotFound("Setting not found");

            settingToUpdate.MaxCourseInUniversity = model.MaxCourseInUniversity;
            settingToUpdate.FirtsSemesterStart = model.FirtsSemesterStart;
            settingToUpdate.FirtsSemesterEnd = model.FirtsSemesterEnd;
            settingToUpdate.SecondSemesterStart = model.SecondSemesterStart;
            settingToUpdate.SecondSemesterEnd = model.SecondSemesterEnd;
            settingToUpdate.PrepareToUpdate(_identityService);

            _db.Settings.Update(settingToUpdate);
            await _db.SaveChangesAsync();

            return Result<SettingViewModel>.SuccessWithData(_mapper.Map<SettingViewModel>(settingToUpdate));
        }
    }
}