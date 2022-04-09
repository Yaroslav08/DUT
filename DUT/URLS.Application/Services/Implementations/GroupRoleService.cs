using AutoMapper;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Group.GroupMember;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
namespace URLS.Application.Services.Implementations
{
    public class GroupRoleService : BaseService<UserGroupRole>, IGroupRoleService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public GroupRoleService(URLSDbContext db, IMapper mapper, IIdentityService identityService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<List<UserGroupRoleViewModel>>> GetAllGroupRolesAsync()
        {
            return Result<List<UserGroupRoleViewModel>>.SuccessWithData(
                _mapper.Map<List<UserGroupRoleViewModel>>(await _db.UserGroupRoles.AsNoTracking().ToListAsync()));
        }
    }
}