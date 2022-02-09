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
    public class RoleClaimsService : BaseService<Role>, IRoleClaimsService
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly DUTDbContext _db;
        public RoleClaimsService(IIdentityService identityService, IMapper mapper, DUTDbContext db) : base(db)
        {
            _identityService = identityService;
            _mapper = mapper;
            _db = db;
        }

        public async Task<Result<RoleViewModel>> CreateRoleAsync(RoleCreateModel model)
        {
            if (await IsExistAsync(s => s.Name == model.Name && s.ClaimsHash == model.ClaimsIds.GetHashForClaimIds()))
            {
                return Result<RoleViewModel>.Error("Same role already exist");
            }
            var isAllClaimsExist = await CheckClaimsAsync(model.ClaimsIds);
            if (!isAllClaimsExist)
            {
                return Result<RoleViewModel>.Error("Not all claims exist");
            }

            var newRole = new Role
            {
                Name = model.Name,
                CountClaims = model.ClaimsIds.Count(),
                ClaimsHash = model.ClaimsIds.GetHashForClaimIds()
            };
            newRole.PrepareToCreate(_identityService);

            await _db.Roles.AddAsync(newRole);
            await _db.SaveChangesAsync();

            var roleClaims = model.ClaimsIds.Select(s => new RoleClaim
            {
                RoleId = newRole.Id,
                ClaimId = s
            }).ToList();

            roleClaims.ForEach(x =>
            {
                x.PrepareToCreate(_identityService);
            });

            await _db.RoleClaims.AddRangeAsync(roleClaims);
            await _db.SaveChangesAsync();

            return Result<RoleViewModel>.SuccessWithData(_mapper.Map<RoleViewModel>(newRole));
        }

        public async Task<Result<List<RoleViewModel>>> GetAllRolesAsync()
        {
            var roles = await _db.Roles.AsNoTracking().ToListAsync();

            var rolesToView = _mapper.Map<List<RoleViewModel>>(roles);

            return Result<List<RoleViewModel>>.SuccessWithData(rolesToView);
        }

        public async Task<Result<List<ClaimViewModel>>> GetClaimsAsync()
        {
            var roles = await _db.Claims.AsNoTracking().ToListAsync();

            var rolesToView = _mapper.Map<List<ClaimViewModel>>(roles);

            return Result<List<ClaimViewModel>>.SuccessWithData(rolesToView);
        }

        public async Task<Result<RoleViewModel>> GetRoleByIdAsync(int id, bool withClaims = false)
        {
            var role = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (role == null)
                return Result<RoleViewModel>.NotFound("Role not found");

            var roleToView = _mapper.Map<RoleViewModel>(role);

            if (withClaims)
            {
                var roleClaims = await _db.RoleClaims.AsNoTracking().Include(s => s.Claim).Where(s => s.RoleId == id).ToListAsync();
                if (roleClaims == null || !roleClaims.Any())
                {
                    roleToView.Claims = null;
                }
                else
                {
                    roleToView.ClaimIds = roleClaims.Select(x => x.Claim.Id).ToArray();
                    roleToView.Claims = _mapper.Map<List<ClaimViewModel>>(roleClaims.Select(s => s.Claim));
                }
            }
            return Result<RoleViewModel>.SuccessWithData(roleToView);
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

        public async Task<Result<RoleViewModel>> UpdateRoleAsync(RoleEditModel model)
        {
            var roleToUpdate = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(s => s.Id == model.Id);

            if (roleToUpdate == null)
                return Result<RoleViewModel>.NotFound("Role not found");

            if (!CheckIsNeedToUpdateRole(roleToUpdate, model))
                return Result<RoleViewModel>.SuccessWithData(_mapper.Map<RoleViewModel>(roleToUpdate));

            var isAllClaimsExist = await CheckClaimsAsync(model.ClaimsIds);
            if (!isAllClaimsExist)
            {
                return Result<RoleViewModel>.Error("Not all claims exist");
            }

            var roleClaims = await _db.RoleClaims.AsNoTracking().Where(s => s.RoleId == model.Id).ToListAsync();

            _db.RoleClaims.RemoveRange(roleClaims);
            await _db.SaveChangesAsync();

            var newRoleClaims = model.ClaimsIds.Select(x => new RoleClaim
            {
                ClaimId = x,
                RoleId = model.Id,
            }).ToList();

            newRoleClaims.ForEach(s =>
            {
                s.PrepareToCreate(_identityService);
            });

            roleToUpdate.CountClaims = newRoleClaims.Count;
            roleToUpdate.ClaimsHash = model.ClaimsIds.GetHashForClaimIds();

            await _db.RoleClaims.AddRangeAsync(newRoleClaims);
            await _db.SaveChangesAsync();

            return Result<RoleViewModel>.SuccessWithData(_mapper.Map<RoleViewModel>(roleToUpdate));
        }

        private bool CheckIsNeedToUpdateRole(Role currentRole, RoleEditModel updatedRole)
        {
            var countOfNewRoleClaims = updatedRole.ClaimsIds.Count();
            var hash = updatedRole.ClaimsIds.GetHashForClaimIds();
            return countOfNewRoleClaims == currentRole.CountClaims
                && hash == currentRole.ClaimsHash;
        }

        private async Task<bool> CheckClaimsAsync(int[] claimsIds)
        {
            var claims = await _db.Claims.AsNoTracking().Where(s => claimsIds.Contains(s.Id)).ToListAsync();
            return claims.Count == claimsIds.Count();
        }

        public async Task<Result<RoleViewModel>> RemoveRoleAsync(int id)
        {
            var roleToRemove = await _db.Roles.FindAsync(id);
            if (roleToRemove == null)
                return Result<RoleViewModel>.NotFound("Role not found");

            var roleClaims = await _db.RoleClaims.Where(s => s.RoleId == id).ToListAsync();
            if(roleClaims.Any())
            {
                _db.RoleClaims.RemoveRange(roleClaims);
                await _db.SaveChangesAsync();
            }
            _db.Roles.Remove(roleToRemove);
            await _db.SaveChangesAsync();
            return Result<RoleViewModel>.Success();
        }
    }
}