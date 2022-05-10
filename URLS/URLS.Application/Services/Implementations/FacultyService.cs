using AutoMapper;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Faculty;
using URLS.Application.ViewModels.Specialty;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using URLS.Constants.Extensions;

namespace URLS.Application.Services.Implementations
{
    public class FacultyService : BaseService<Faculty>, IFacultyService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public FacultyService(URLSDbContext db, IMapper mapper, IIdentityService identityService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<FacultyViewModel>> CreateFacultyAsync(FacultyCreateModel model)
        {
            if (await IsExistAsync(x => x.Name == model.Name))
                return Result<FacultyViewModel>.Error("Faculty already exist");
            var faculty = new Faculty
            {
                Name = model.Name,
                UniversityId = 1
            };
            faculty.PrepareToCreate(_identityService);
            await _db.Faculties.AddAsync(faculty);
            await _db.SaveChangesAsync();
            return Result<FacultyViewModel>.SuccessWithData(_mapper.Map<FacultyViewModel>(faculty));
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
            return Result<List<FacultyViewModel>>.SuccessWithData(await _db.Faculties.AsNoTracking().Select(x => new FacultyViewModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Name = x.Name
            }).ToListAsync());
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
            return Result<List<FacultyViewModel>>.SuccessWithData(await _db.Faculties.AsNoTracking().Where(s => s.UniversityId == id).Select(x => new FacultyViewModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Name = x.Name
            }).ToListAsync());
        }
    }
}