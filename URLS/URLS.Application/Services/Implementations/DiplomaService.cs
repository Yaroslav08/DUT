using AutoMapper;
using Extensions.Generator;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Diploma;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class DiplomaService : IDiplomaService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICommonService _commonService;
        public DiplomaService(URLSDbContext db, IMapper mapper, IIdentityService identityService, ICommonService commonService)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
            _commonService = commonService;
        }

        public async Task<Result<List<DiplomaViewModel>>> GetDiplomaTemplatesAsync()
        {
            var userDiplomas = await _db.Diplomas.AsNoTracking().Where(x => x.IsTemplate).ToListAsync();
            return Result<List<DiplomaViewModel>>.SuccessWithData(_mapper.Map<List<DiplomaViewModel>>(userDiplomas));
        }

        public async Task<Result<List<DiplomaViewModel>>> GetUserDiplomasAsync(int userId)
        {
            var userDiplomas = await _db.Diplomas.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();
            return Result<List<DiplomaViewModel>>.SuccessWithData(_mapper.Map<List<DiplomaViewModel>>(userDiplomas));
        }

        public async Task<Result<bool>> CreateTemplatesAutomaticallyAsync()
        {
            string[] diplomaNames = new string[] { "ДИПЛОМ МОЛОДШОГО СПЕЦІАЛІСТА", "ДИПЛОМ БАКАЛАВРА", "ДИПЛОМ МАГІСТРА" };

            var university = await _db.Universities.FirstOrDefaultAsync();
            var specialties = await _db.Specialties.ToListAsync();
            var setting = await _db.Settings.FirstOrDefaultAsync();

            var diplomas = new List<Diploma>();

            foreach (var specialty in specialties)
            {
                foreach (var diplomaName in diplomaNames)
                {
                    var newDiploma = new Diploma
                    {
                        Id = RandomGenerator.GetUniqCode().ToUpper(),
                        IsTemplate = true,
                        Name = diplomaName,
                        Specialty = specialty.Name,
                        DirectorSignature = setting.DirectorSignature,
                        UserId = null,
                        EndedAt = DateTime.Today,
                        Number = new Random().Next(1000, 9999),
                        Qualification = "",
                        Series = "ET",
                        Student = "Бандера Степан Андрійович",
                        University = university.Name.ToUpper(),
                        UniversityStamp = setting.UniversityStamp
                    };
                }
            }
            await _db.Diplomas.AddRangeAsync(diplomas);
            await _db.SaveChangesAsync();
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
                return Result<DiplomaViewModel>.NotFound(typeof(Diploma).NotFoundMessage(model.Id));

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
                return Result<bool>.NotFound(typeof(Diploma).NotFoundMessage(diplomaId));
            _db.Diplomas.Remove(diploma);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<DiplomaViewModel>> CreateDiplomaBasicOnTemplateAsync(DiplomaCreateModel model, string templateId)
        {
            var existDiploma = await _commonService.IsExistWithResultsAsync<Diploma>(x => x.Number == model.Number && x.Series == model.Series);
            if (existDiploma.IsExist)
                return Result<DiplomaViewModel>.Error("Diploma is already created");

            existDiploma = await _commonService.IsExistWithResultsAsync<Diploma>(s => s.Id == templateId);
            if (!existDiploma.IsExist)
                return Result<DiplomaViewModel>.NotFound(typeof(Diploma).NotFoundMessage(templateId));

            var studentDiploma = existDiploma.Results.First();

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
                return Result<DiplomaViewModel>.NotFound(typeof(Diploma).NotFoundMessage(id));
            return Result<DiplomaViewModel>.SuccessWithData(_mapper.Map<DiplomaViewModel>(diploma));
        }

        public async Task<Result<DiplomaViewModel>> GetDiplomaTemplateByIdAsync(string id)
        {
            var diploma = await _db.Diplomas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IsTemplate);
            if (diploma == null)
                return Result<DiplomaViewModel>.NotFound(typeof(Diploma).NotFoundMessage(id));
            return Result<DiplomaViewModel>.SuccessWithData(_mapper.Map<DiplomaViewModel>(diploma));
        }
    }
}