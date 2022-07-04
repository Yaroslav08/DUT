using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Specialty;
using URLS.Constants;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly IFacultyService _faсultyService;
        private readonly ICommonService _commonService;
        public SpecialtyService(URLSDbContext db, IMapper mapper, IIdentityService identityService, IFacultyService faсultyService, ICommonService commonService)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
            _faсultyService = faсultyService;
            _commonService = commonService;
        }

        public async Task<Result<SpecialtyViewModel>> CreateSpecialtyAsync(SpecialtyCreateModel model)
        {
            if (await _commonService.IsExistAsync<Specialty>(x => x.Name == model.Name && x.Code == model.Code))
                return Result<SpecialtyViewModel>.Error("Specialty already exist");
            var currentFaculty = await _faсultyService.GetFacultyByIdAsync(model.FacultyId);
            if (currentFaculty.IsNotFound)
                return Result<SpecialtyViewModel>.NotFound("Faculty not found");
            var newSpecialty = new Specialty
            {
                Name = model.Name,
                Code = model.Code,
                Invite = Generator.CreateGroupInviteCode(),
                FacultyId = model.FacultyId
            };
            newSpecialty.PrepareToCreate(_identityService);
            await _db.Specialties.AddAsync(newSpecialty);
            await _db.SaveChangesAsync();

            return Result<SpecialtyViewModel>.Created(_mapper.Map<SpecialtyViewModel>(newSpecialty));
        }

        public async Task<Result<List<SpecialtyViewModel>>> GetAllSpecialtiesAsync()
        {
            return Result<List<SpecialtyViewModel>>.SuccessList(await _db.Specialties.AsNoTracking().Select(x => new SpecialtyViewModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Name = x.Name,
                Code = x.Code
            }).ToListAsync());
        }

        public async Task<Result<string>> GetInviteAsync(int specialtyId)
        {
            var specialty = await _db.Specialties.AsNoTracking().FirstOrDefaultAsync(s => s.Id == specialtyId);
            if (specialty == null)
                return Result<string>.NotFound(typeof(Specialty).NotFoundMessage(specialtyId));

            return Result<string>.SuccessWithData(specialty.Invite);
        }

        public async Task<Result<List<SpecialtyViewModel>>> GetSpecialtiesByFacultyIdAsync(int facultyId)
        {
            return Result<List<SpecialtyViewModel>>.SuccessList(await _db.Specialties.AsNoTracking().Where(x => x.FacultyId == facultyId).Select(x => new SpecialtyViewModel
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
            }).FirstOrDefaultAsync(x => x.Id == id);
            if (specialty == null)
                return Result<SpecialtyViewModel>.NotFound();
            return Result<SpecialtyViewModel>.SuccessWithData(specialty);
        }

        public async Task<Result<string>> UpdateInviteAsync(int specialtyId)
        {
            var specialtyForUpdate = await _db.Specialties.AsNoTracking().FirstOrDefaultAsync(s => s.Id == specialtyId);
            if (specialtyForUpdate == null)
                return Result<string>.NotFound(typeof(Specialty).NotFoundMessage(specialtyId));

            specialtyForUpdate.Invite = await GetNewInvite();
            specialtyForUpdate.PrepareToUpdate(_identityService);

            _db.Specialties.Update(specialtyForUpdate);
            await _db.SaveChangesAsync();

            return Result<string>.SuccessWithData(specialtyForUpdate.Invite);
        }

        public async Task<Result<SpecialtyViewModel>> UpdateSpecialtyAsync(SpecialtyEditModel model)
        {
            var currentSpecialty = await _db.Specialties.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.Id);
            if (currentSpecialty == null)
                return Result<SpecialtyViewModel>.NotFound();
            var faculty = await _db.Faculties.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.FacultyId);
            if (faculty == null)
                return Result<SpecialtyViewModel>.NotFound("Facuty not found");
            currentSpecialty.Name = model.Name;
            currentSpecialty.Code = model.Code;
            currentSpecialty.FacultyId = model.FacultyId;
            currentSpecialty.PrepareToUpdate(_identityService);
            _db.Specialties.Update(currentSpecialty);
            await _db.SaveChangesAsync();
            return Result<SpecialtyViewModel>.SuccessWithData(_mapper.Map<SpecialtyViewModel>(currentSpecialty));
        }

        public async Task<Result<List<SpecialtyTeacherViewModel>>> GetSpecialtyTeachersAsync(int specialtyId, int offset, int count)
        {
            if (!await _commonService.IsExistAsync<Specialty>(s => s.Id == specialtyId))
                return Result<List<SpecialtyTeacherViewModel>>.NotFound(typeof(Specialty).NotFoundMessage(specialtyId));

            var teachers = await _db.UserSpecialties
                .AsNoTracking()
                .Where(s => s.SpecialtyId == specialtyId)
                .Include(s => s.User)
                .OrderBy(s => s.User.LastName)
                .Skip(offset).Take(count)
                .ToListAsync();

            return Result<List<SpecialtyTeacherViewModel>>.SuccessWithData(_mapper.Map<List<SpecialtyTeacherViewModel>>(teachers));
        }

        public async Task<Result<SpecialtyTeacherViewModel>> CreateSpecialtyTeacherAsync(SpecialtyTeacherCreateModel createModel)
        {
            if (!await _commonService.IsExistAsync<Specialty>(s => s.Id == createModel.SpecialtyId))
                return Result<SpecialtyTeacherViewModel>.NotFound(typeof(Specialty).NotFoundMessage(createModel.SpecialtyId));

            if (await _commonService.IsExistAsync<UserSpecialty>(s => s.SpecialtyId == createModel.SpecialtyId && s.UserId == createModel.TeacherId))
                return Result<SpecialtyTeacherViewModel>.Error("This teacher is already member of this specialty");

            var newTeacherSpecialty = new UserSpecialty
            {
                SpecialtyId = createModel.SpecialtyId,
                UserId = createModel.TeacherId,
                Title = createModel.Title
            };

            newTeacherSpecialty.PrepareToCreate(_identityService);
            await _db.UserSpecialties.AddAsync(newTeacherSpecialty);
            await _db.SaveChangesAsync();

            return Result<SpecialtyTeacherViewModel>.SuccessWithData(_mapper.Map<SpecialtyTeacherViewModel>(newTeacherSpecialty));
        }

        public async Task<Result<SpecialtyTeacherViewModel>> UpdateSpecialtyTeacherAsync(SpecialtyTeacherEditModel editModel)
        {
            var teacherSpecialtyForUpdate = await _db.UserSpecialties.AsNoTracking().FirstOrDefaultAsync(s => s.Id == editModel.Id);
            if (teacherSpecialtyForUpdate == null)
                return Result<SpecialtyTeacherViewModel>.NotFound(typeof(UserSpecialty).NotFoundMessage(editModel.Id));

            teacherSpecialtyForUpdate.Title = editModel.Title;
            teacherSpecialtyForUpdate.PrepareToUpdate(_identityService);
            _db.UserSpecialties.Update(teacherSpecialtyForUpdate);
            await _db.SaveChangesAsync();

            return Result<SpecialtyTeacherViewModel>.SuccessWithData(_mapper.Map<SpecialtyTeacherViewModel>(teacherSpecialtyForUpdate));
        }

        public async Task<Result<bool>> RemoveSpecialtyTeacherAsync(int specialtyTeacherId)
        {
            var specialtyTeacherForDelete = await _db.UserSpecialties.FirstOrDefaultAsync(s => s.Id == specialtyTeacherId);
            if (specialtyTeacherForDelete == null)
                return Result<bool>.NotFound(typeof(UserSpecialty).NotFoundMessage(specialtyTeacherId));

            _db.UserSpecialties.Remove(specialtyTeacherForDelete);
            await _db.SaveChangesAsync();

            return Result<bool>.Success();
        }

        private async Task<string> GetNewInvite()
        {
            string invite = Generator.CreateGroupInviteCode();

            while (await _db.Specialties.AnyAsync(s => s.Invite == invite))
            {
                invite = Generator.CreateGroupInviteCode();
            }

            return invite;
        }
    }
}