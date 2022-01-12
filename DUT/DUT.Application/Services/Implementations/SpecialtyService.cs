using AutoMapper;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Faculty;
using DUT.Application.ViewModels.Specialty;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class SpecialtyService : BaseService<Specialty>, ISpecialtyService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly IFacultyService _faultyService;
        public SpecialtyService(DUTDbContext db, IMapper mapper, IIdentityService identityService, IFacultyService faultyService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
            _faultyService = faultyService;
        }

        public async Task<Result<SpecialtyViewModel>> CreateSpecialtyAsync(SpecialtyCreateModel model)
        {
            if (await IsExistAsync(x => x.Name == model.Name && x.Code == model.Code))
                return Result<SpecialtyViewModel>.Error("Specialty already exist");
            var currentFaculty = await _faultyService.GetFacultyByIdAsync(model.FacultyId);
            if (currentFaculty.IsNotFound)
                return Result<SpecialtyViewModel>.NotFound("Faculty not found");
            var newSpecialty = new Specialty
            {
                CreatedAt = DateTime.Now,
                CreatedBy = _identityService.GetIdentityData(),
                CreatedFromIP = model.IP,
                Code = model.Code,
                Name = model.Name,
                FacultyId = model.FacultyId
            };
            await _db.Specialties.AddAsync(newSpecialty);
            await _db.SaveChangesAsync();

            return Result<SpecialtyViewModel>.SuccessWithData(_mapper.Map<SpecialtyViewModel>(newSpecialty));

        }

        public async Task<Result<List<SpecialtyViewModel>>> GetAllSpecialtiesAsync()
        {
            return Result<List<SpecialtyViewModel>>.SuccessWithData(await _db.Specialties.AsNoTracking().Select(x => new SpecialtyViewModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Name = x.Name,
                Code = x.Code
            }).ToListAsync());
        }

        public async Task<Result<List<SpecialtyViewModel>>> GetSpecialtiesByFacultyIdAsync(int facultyId)
        {
            return Result<List<SpecialtyViewModel>>.SuccessWithData(await _db.Specialties.AsNoTracking().Where(x => x.FacultyId == facultyId).Select(x => new SpecialtyViewModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Name = x.Name,
                Code = x.Code
            }).ToListAsync());
        }

        public async Task<Result<SpecialtyViewModel>> GetSpecialtyByIdAsync(int id)
        {
            var specialty = await _db.Specialties.AsNoTracking().Select(x => new SpecialtyViewModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Name = x.Name,
                Code = x.Code
            }).SingleOrDefaultAsync(x => x.Id == id);
            if (specialty == null)
                return Result<SpecialtyViewModel>.NotFound();
            return Result<SpecialtyViewModel>.SuccessWithData(specialty);
        }

        public async Task<Result<SpecialtyViewModel>> UpdateSpecialtyAsync(SpecialtyEditModel model)
        {
            var currentSpecialty = await _db.Specialties.AsNoTracking().SingleOrDefaultAsync(x => x.Id == model.Id);
            if (currentSpecialty == null)
                return Result<SpecialtyViewModel>.NotFound();
            var faculty = await _db.Faculties.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.FacultyId);
            if (faculty == null)
                return Result<SpecialtyViewModel>.NotFound("Facuty not found");
            currentSpecialty.Name = model.Name;
            currentSpecialty.Code = model.Code;
            currentSpecialty.FacultyId = model.FacultyId;
            currentSpecialty.LastUpdatedAt = DateTime.Now;
            currentSpecialty.LastUpdatedBy = _identityService.GetIdentityData();
            currentSpecialty.LastUpdatedFromIP = model.IP;
            _db.Specialties.Update(currentSpecialty);
            await _db.SaveChangesAsync();
            return Result<SpecialtyViewModel>.SuccessWithData(_mapper.Map<SpecialtyViewModel>(currentSpecialty));
        }
    }
}