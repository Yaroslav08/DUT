using AutoMapper;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Group.GroupMember;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
namespace DUT.Application.Services.Implementations
{
    public class GroupRoleService : BaseService<UserGroupRole>, IGroupRoleService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public GroupRoleService(DUTDbContext db, IMapper mapper, IIdentityService identityService) : base(db)
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