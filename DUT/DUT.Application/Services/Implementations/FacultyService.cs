using AutoMapper;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Faculty;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class FacultyService : IFacultyService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public FacultyService(DUTDbContext db, IMapper mapper, IIdentityService identityService)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<FacultyViewModel>> CreateFacultyAsync(FacultyCreateModel model)
        {
            var existFaculty = await _db.Faculties.AsNoTracking().AnyAsync(x => x.Name == model.Name);
            if (existFaculty)
                return Result<FacultyViewModel>.Error("Faculty already exist");
            var faculty = new Faculty
            {
                CreatedAt = DateTime.Now,
                CreatedBy = _identityService.GetIdentityData(),
                CreatedFromIP = model.IP,
                Name = model.Name,
                UniversityId = 1
            };
            await _db.Faculties.AddAsync(faculty);
            await _db.SaveChangesAsync();
            return Result<FacultyViewModel>.SuccessWithData(_mapper.Map<FacultyViewModel>(faculty));
        }

        public async Task<Result<FacultyViewModel>> EditFacultyAsync(FacultyEditModel model)
        {
            var currentFaculty = await _db.Faculties.AsNoTracking().SingleOrDefaultAsync(x => x.Id == model.Id);
            if (currentFaculty == null)
                return Result<FacultyViewModel>.NotFound();
            currentFaculty.Name = model.Name;
            currentFaculty.LastUpdatedAt = DateTime.Now;
            currentFaculty.LastUpdatedBy = _identityService.GetIdentityData();
            currentFaculty.LastUpdatedFromIP = model.IP;
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
            }).SingleOrDefaultAsync(x => x.Id == id);
            if (faculty == null)
                return Result<FacultyViewModel>.NotFound();
            return Result<FacultyViewModel>.SuccessWithData(faculty);
        }
    }
}