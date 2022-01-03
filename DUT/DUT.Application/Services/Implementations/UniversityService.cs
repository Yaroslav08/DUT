using AutoMapper;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.University;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class UniversityService : IUniversityService
    {
        private readonly IMapper _mapper;
        private readonly DUTDbContext _db;
        public UniversityService(IMapper mapper, DUTDbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<Result<UniversityViewModel>> CreateUniversityAsync(UniversityCreateModel model)
        {
            var newUniversity = _mapper.Map<University>(model);

            await _db.Universities.AddAsync(newUniversity);
            await _db.SaveChangesAsync();
            return Result<UniversityViewModel>.SuccessWithData(_mapper.Map<UniversityViewModel>(newUniversity));
        }

        public async Task<Result<UniversityViewModel>> GetUniversityAsync()
        {
            var university = await _db.Universities.AsNoTracking().FirstOrDefaultAsync();
            if (university == null)
                return Result<UniversityViewModel>.NotFound();
            var universityViewModel = _mapper.Map<UniversityViewModel>(university);
            return Result<UniversityViewModel>.SuccessWithData(universityViewModel);
        }

        public async Task<Result<UniversityViewModel>> UpdateUniversityAsync(UniversityEditModel model)
        {
            var updatedUniversity = _mapper.Map<University>(model);

            _db.Universities.Update(updatedUniversity);
            await _db.SaveChangesAsync();
            return Result<UniversityViewModel>.SuccessWithData(_mapper.Map<UniversityViewModel>(updatedUniversity));
        }
    }
}