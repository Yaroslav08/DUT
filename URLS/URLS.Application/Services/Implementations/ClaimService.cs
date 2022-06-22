using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.RoleClaim;
using URLS.Constants.APIResponse;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class ClaimService : IClaimService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public ClaimService(URLSDbContext db, IMapper mapper, IIdentityService identityService)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<List<ClaimViewModel>>> GetAllClaimsAsync(int offset = 0, int limit = 100)
        {
            var roles = await _db.Claims.AsNoTracking().Skip(offset).Take(limit).ToListAsync();

            var rolesToView = _mapper.Map<List<ClaimViewModel>>(roles);

            var count = await _db.Claims.CountAsync();

            return Result<List<ClaimViewModel>>.SuccessList(rolesToView, Meta.FromMeta(count, offset, limit));
        }

        public async Task<Result<List<ClaimViewModel>>> GetClaimsByRoleIdAsync(int roleId)
        {
            var roles = await _db.RoleClaims.AsNoTracking().Include(s => s.Claim).Where(s => s.RoleId == roleId).ToListAsync();

            var rolesToView = _mapper.Map<List<ClaimViewModel>>(roles.Select(s => s.Claim));

            var count = await _db.RoleClaims.CountAsync(s => s.RoleId == roleId);

            return Result<List<ClaimViewModel>>.SuccessList(rolesToView, Meta.FromMeta(count, 0,0));
        }

        public async Task<Result<ClaimViewModel>> UpdateClaimAsync(ClaimEditModel model)
        {
            var claimToUpdate = await _db.Claims.FindAsync(model.Id);
            if (claimToUpdate == null)
                return Result<ClaimViewModel>.NotFound(typeof(Claim).NotFoundMessage(model.Id));

            claimToUpdate.DisplayName = model.DisplayName;

            claimToUpdate.PrepareToUpdate(_identityService);
            _db.Claims.Update(claimToUpdate);
            await _db.SaveChangesAsync();
            return Result<ClaimViewModel>.SuccessWithData(_mapper.Map<ClaimViewModel>(claimToUpdate));
        }
    }
}