using AutoMapper;
using DUT.Application.Extensions;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Diploma;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Extensions.Generator;

namespace DUT.Application.Services.Implementations
{
    public class DiplomaService : BaseService<Diploma>, IDiplomaService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public DiplomaService(DUTDbContext db, IMapper mapper, IIdentityService identityService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<List<DiplomaViewModel>>> GetDiplomaTemplatesAsync()
        {
            var userDiplomas = await _db.Diplomas.AsNoTracking().Where(x => x.IsTemplate).ToListAsync();
            if (userDiplomas == null || !userDiplomas.Any())
                return Result<List<DiplomaViewModel>>.Success();
            return Result<List<DiplomaViewModel>>.SuccessWithData(_mapper.Map<List<DiplomaViewModel>>(userDiplomas));
        }

        public async Task<Result<List<DiplomaViewModel>>> GetUserDiplomasAsync(int userId)
        {
            var userDiplomas = await _db.Diplomas.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();
            if (userDiplomas == null || !userDiplomas.Any())
                return Result<List<DiplomaViewModel>>.Success();
            return Result<List<DiplomaViewModel>>.SuccessWithData(_mapper.Map<List<DiplomaViewModel>>(userDiplomas));
        }

        public async Task<Result<bool>> CreateTemplatesAutomaticallyAsync()
        {
            string[] diplomaNames = new string[] { "ДИПЛОМ МОЛОДШОГО СПЕЦІАЛІСТА", "ДИПЛОМ БАКАЛАВРА", "ДИПЛОМ МАГІСТРА" };

            return Result<bool>.Success();
        }

        public async Task<Result<DiplomaViewModel>> CreateDiplomaTemplateAsync(DiplomaTemplateCreateModel model)
        {
            var templateDiploma = new Diploma
            {
                Id = RandomGenerator.GetUniqCode().ToUpper(),
                Name = model.Name,
                DirectorSignature = model.DirectorSignaturePath,
                EndedAt = model.EndedAt,
                IsTemplate = true,
                Number = model.Number,
                Qualification = model.Qualification,
                Series = model.Series,
                Specialty = model.Specialty,
                Student = model.Student,
                University = model.University,
                UniversityStamp = model.UniversityStampPath
            };
            templateDiploma.PrepareToCreate(_identityService);
            await _db.Diplomas.AddAsync(templateDiploma);
            await _db.SaveChangesAsync();
            return Result<DiplomaViewModel>.SuccessWithData(_mapper.Map<DiplomaViewModel>(templateDiploma));
        }

        public async Task<Result<DiplomaViewModel>> UpdateDiplomaTemplateAsync(DiplomaTemplateEditModel model)
        {
            var templateToUpdate = await _db.Diplomas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.Id && x.IsTemplate);
            if (templateToUpdate == null)
                return Result<DiplomaViewModel>.NotFound("Diploma not found");

            templateToUpdate.Name = model.Name;
            templateToUpdate.DirectorSignature = model.DirectorSignaturePath;
            templateToUpdate.EndedAt = model.EndedAt;
            templateToUpdate.IsTemplate = true;
            templateToUpdate.Number = model.Number;
            templateToUpdate.Qualification = model.Qualification;
            templateToUpdate.Series = model.Series;
            templateToUpdate.Specialty = model.Specialty;
            templateToUpdate.Student = model.Student;
            templateToUpdate.University = model.University;
            templateToUpdate.UniversityStamp = model.UniversityStampPath;
            templateToUpdate.PrepareToUpdate(_identityService);
            _db.Diplomas.Update(templateToUpdate);
            await _db.SaveChangesAsync();
            return Result<DiplomaViewModel>.SuccessWithData(_mapper.Map<DiplomaViewModel>(templateToUpdate));
        }

        public async Task<Result<bool>> RemoveDiplomaAsync(string diplomaId)
        {
            var diploma = await _db.Diplomas.FindAsync(diplomaId);
            if (diploma == null)
                return Result<bool>.NotFound("Diploma not found");
            _db.Diplomas.Remove(diploma);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<DiplomaViewModel>> CreateDiplomaBasicOnTemplateAsync(DiplomaCreateModel model, string templateId)
        {
            if (!await IsExistAsync(s => s.Id == templateId))
                return Result<DiplomaViewModel>.NotFound("Diploma template not found");

            if (await IsExistAsync(x => x.Number == model.Number && x.Series == model.Series))
                return Result<DiplomaViewModel>.Error("Diploma is already created");

            var studentDiploma = Exists.First();

            var userGroup = await _db.UserGroups
                .AsNoTracking()
                .Include(x => x.Group)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == _identityService.GetUserId() && x.Status == UserGroupStatus.Member);

            var group = userGroup.Group;
            var student = userGroup.User;

            studentDiploma.Id = RandomGenerator.GetUniqCode().ToUpper();
            studentDiploma.Student = $"{student.LastName} {student.FirstName} {student.MiddleName}";
            studentDiploma.EndedAt = group.EndStudy;
            studentDiploma.Series = model.Series;
            studentDiploma.Number = model.Number;
            studentDiploma.IsTemplate = false;
            studentDiploma.UserId = _identityService.GetUserId();

            studentDiploma.PrepareToCreate(_identityService);

            await _db.Diplomas.AddAsync(studentDiploma);
            await _db.SaveChangesAsync();

            return Result<DiplomaViewModel>.SuccessWithData(_mapper.Map<DiplomaViewModel>(studentDiploma));
        }

        public async Task<Result<DiplomaViewModel>> GetDiplomaByIdAsync(string id)
        {
            var diploma = await _db.Diplomas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (diploma == null)
                return Result<DiplomaViewModel>.NotFound("Diploma not found");
            return Result<DiplomaViewModel>.SuccessWithData(_mapper.Map<DiplomaViewModel>(diploma));
        }

        public async Task<Result<DiplomaViewModel>> GetDiplomaTemplateByIdAsync(string id)
        {
            var diploma = await _db.Diplomas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IsTemplate);
            if (diploma == null)
                return Result<DiplomaViewModel>.NotFound("Diploma not found");
            return Result<DiplomaViewModel>.SuccessWithData(_mapper.Map<DiplomaViewModel>(diploma));
        }
    }
}