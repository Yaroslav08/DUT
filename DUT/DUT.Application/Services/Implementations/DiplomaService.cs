using AutoMapper;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Diploma;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class DiplomaService : IDiplomaService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        public readonly IIdentityService _identityService;
        public DiplomaService(DUTDbContext db, IMapper mapper, IIdentityService identityService)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<List<DiplomaViewModel>>> GetUserDiplomasAsync(int userId)
        {
            var userDiplomas = await _db.Diplomas.AsNoTracking().Where(s => s.UserId == userId).ToListAsync();
            return Result<List<DiplomaViewModel>>.SuccessWithData(_mapper.Map<List<DiplomaViewModel>>(userDiplomas));
        }

        public async Task<Result<DiplomaViewModel>> OrderDiplomaAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<DiplomaViewModel>> ReorderDiplomaAsync(int userId, string diplomaId)
        {
            throw new NotImplementedException();
        }
    }
}
