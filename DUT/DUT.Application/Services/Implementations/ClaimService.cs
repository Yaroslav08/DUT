using AutoMapper;
using DUT.Application.Extensions;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.RoleClaim;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class ClaimService : BaseService<Claim>, IClaimService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public ClaimService(DUTDbContext db, IMapper mapper, IIdentityService identityService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<List<ClaimViewModel>>> GetAllClaimsAsync()
        {
            var roles = await _db.Claims.AsNoTracking().ToListAsync();

            var rolesToView = _mapper.Map<List<ClaimViewModel>>(roles);

            return Result<List<ClaimViewModel>>.SuccessWithData(rolesToView);
        }

        public async Task<Result<List<ClaimViewModel>>> GetClaimsByRoleIdAsync(int roleId)
        {
            var roles = await _db.RoleClaims.AsNoTracking().Include(s => s.Claim).Where(s => s.RoleId == roleId).ToListAsync();

            var rolesToView = _mapper.Map<List<ClaimViewModel>>(roles.Select(s => s.Claim));

            return Result<List<ClaimViewModel>>.SuccessWithData(rolesToView);
        }

        public async Task<Result<ClaimViewModel>> UpdateClaimAsync(ClaimEditModel model)
        {
            var claimToUpdate = await _db.Claims.FindAsync(model.Id);
            if (claimToUpdate == null)
                return Result<ClaimViewModel>.NotFound("Claim not found");

            claimToUpdate.DisplayName = model.DisplayName;

            claimToUpdate.PrepareToUpdate(_identityService);
            _db.Claims.Update(claimToUpdate);
            await _db.SaveChangesAsync();
            return Result<ClaimViewModel>.SuccessWithData(_mapper.Map<ClaimViewModel>(claimToUpdate));
        }
    }
}
