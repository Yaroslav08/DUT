using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Faculty;
using URLS.Constants.APIResponse;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class FacultyService : IFacultyService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICommonService _commonService;
        public FacultyService(URLSDbContext db, IMapper mapper, IIdentityService identityService, ICommonService commonService)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
            _commonService = commonService;
        }

        public async Task<Result<FacultyViewModel>> CreateFacultyAsync(FacultyCreateModel model)
        {
            if (await _commonService.IsExistAsync<Faculty>(x => x.Name == model.Name))
                return Result<FacultyViewModel>.Error("Faculty already exist");
            var faculty = new Faculty
            {
                Name = model.Name,
                UniversityId = 1
            };
            faculty.PrepareToCreate(_identityService);
            await _db.Faculties.AddAsync(faculty);
            await _db.SaveChangesAsync();
            return Result<FacultyViewModel>.Created(_mapper.Map<FacultyViewModel>(faculty));
        }

        public async Task<Result<FacultyViewModel>> UpdateFacultyAsync(FacultyEditModel model)
        {
            var currentFaculty = await _db.Faculties.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.Id);
            if (currentFaculty == null)
                return Result<FacultyViewModel>.NotFound(typeof(Faculty).NotFoundMessage(model.Id));
            currentFaculty.Name = model.Name;
            currentFaculty.PrepareToUpdate(_identityService);
            _db.Faculties.Update(currentFaculty);
            await _db.SaveChangesAsync();
            return Result<FacultyViewModel>.SuccessWithData(_mapper.Map<FacultyViewModel>(currentFaculty));
        }

        public async Task<Result<List<FacultyViewModel>>> GetAllFacultiesAsync()
        {
            var faculties = await _db.Faculties.AsNoTracking().Select(x => new FacultyViewModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Name = x.Name
            }).ToListAsync();

            var totalCount = await _db.Faculties.CountAsync();

            return Result<List<FacultyViewModel>>.SuccessList(faculties, Meta.FromMeta(totalCount, 0, 0));
        }

        public async Task<Result<FacultyViewModel>> GetFacultyByIdAsync(int id)
        {
            var faculty = await _db.Faculties.AsNoTracking().Select(x => new FacultyViewModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Name = x.Name
            }).FirstOrDefaultAsync(x => x.Id == id);
            if (faculty == null)
                return Result<FacultyViewModel>.NotFound(typeof(Faculty).NotFoundMessage(id));
            return Result<FacultyViewModel>.SuccessWithData(faculty);
        }

        public async Task<Result<List<FacultyViewModel>>> GetFacultiesByUniversityIdAsync(int id)
        {
            var faculties = await _db.Faculties.AsNoTracking().Where(s => s.UniversityId == id).Select(x => new FacultyViewModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Name = x.Name
            }).ToListAsync();

            var totalCount = await _db.Faculties.CountAsync(s => s.UniversityId == id);

            return Result<List<FacultyViewModel>>.SuccessList(faculties, Meta.FromMeta(totalCount, 0, 0));
        }
    }
}