using AutoMapper;
using DUT.Application.Extensions;
using DUT.Application.Options;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Subject;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class SubjectService : BaseService<Subject>, ISubjectService, IBaseService<Subject>
    {
        private readonly IIdentityService _identityService;
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly DUTDbContext _db;

        public SubjectService(IIdentityService identityService, IGroupService groupService, IUserService userService, IMapper mapper, DUTDbContext db) : base(db)
        {
            _identityService = identityService;
            _groupService = groupService;
            _userService = userService;
            _mapper = mapper;
            _db = db;
        }

        public async Task<Result<SubjectViewModel>> CreateSubjectAsync(SubjectCreateModel model)
        {
            if (!await _userService.IsExistAsync(s => s.Id == model.TeacherId))
                return Result<SubjectViewModel>.NotFound("Teacher with this ID not found");

            if (model.GroupId != null)
                if (!await _groupService.IsExistAsync(s => s.Id == model.GroupId))
                    return Result<SubjectViewModel>.NotFound($"Group with ID ({model.GroupId}) not found");

            var newSubject = new Subject
            {
                Name = model.Name,
                Description = model.Description,
                From = model.From,
                To = model.To,
                Config = model.Config,
                Semestr = model.Semestr,
                IsTemplate = model.IsTemplate,
                TeacherId = model.TeacherId,
                GroupId = model.GroupId
            };
            newSubject.PrepareToCreate(_identityService);
            await _db.Subjects.AddAsync(newSubject);
            await _db.SaveChangesAsync();
            return Result<SubjectViewModel>.SuccessWithData(_mapper.Map<SubjectViewModel>(newSubject));
        }

        public async Task<Result<List<SubjectViewModel>>> GetAllSubjectsAsync(SearchSubjectOptions options)
        {
            var query = _db.Subjects
                .AsNoTracking()
                .Where(x => x.IsTemplate == options.IsTemplate)
                .OrderBy(x => x.Id)
                .Skip(options.Offset).Take(options.Count);

            if (!string.IsNullOrEmpty(options.Name))
                query = query.Where(s => s.Name.Contains(options.Name));

            var subjects = await query.ToListAsync();

            var subjectsToView = _mapper.Map<List<SubjectViewModel>>(subjects);
            return Result<List<SubjectViewModel>>.SuccessWithData(subjectsToView);
        }

        public async Task<Result<List<SubjectViewModel>>> GetAllTemplatesAsync()
        {
            var templateSubjects = await _db.Subjects.AsNoTracking().Where(x => x.IsTemplate).ToListAsync();
            var templateSubjectsToView = _mapper.Map<List<SubjectViewModel>>(templateSubjects);
            return Result<List<SubjectViewModel>>.SuccessWithData(templateSubjectsToView);
        }

        public async Task<Result<List<SubjectViewModel>>> SearchSubjectsAsync(SearchSubjectOptions options)
        {
            options.PrepareOptions();

            var query = _db.Subjects.AsNoTracking();

            if (options.GroupId != null)
                query = query.Where(s => s.GroupId == options.GroupId);

            if (options.IsTemplate != null)
                query = query.Where(x => x.IsTemplate);

            if (options.Name != null)
                query = query.Where(x => x.Name.Contains(options.Name));

            query = query.Skip(options.Offset).Take(options.Count);

            query = query.OrderBy(x => x.Id);

            var subjects = await query.ToListAsync();
            var subjectsToView = _mapper.Map<List<SubjectViewModel>>(subjects);
            return Result<List<SubjectViewModel>>.SuccessWithData(subjectsToView);
        }

        public async Task<Result<SubjectViewModel>> GetSubjectByIdAsync(int subjectId)
        {
            var subject = await _db.Subjects
                .AsNoTracking()
                .Include(x => x.Teacher)
                .FirstOrDefaultAsync(s => s.Id == subjectId);
            if (subject == null)
                return Result<SubjectViewModel>.NotFound("Subject not found");
            var subjectToView = _mapper.Map<SubjectViewModel>(subject);
            return Result<SubjectViewModel>.SuccessWithData(subjectToView);
        }
    }
}
