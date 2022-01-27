using AutoMapper;
using DUT.Application.Extensions;
using DUT.Application.Options;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.User;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        public UserService(DUTDbContext db, IMapper mapper) : base(db)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<List<UserShortViewModel>>> GetLastUsersAsync(int count)
        {
            var lastUsers = await _db.Users
                .AsNoTracking()
                .OrderByDescending(s => s.JoinAt)
                .Take(count)
                .ToListAsync();
            return Result<List<UserShortViewModel>>.SuccessWithData(_mapper.Map<List<UserShortViewModel>>(lastUsers));
        }

        public async Task<Result<UserViewModel>> GetUserByIdAsync(int id)
        {
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return Result<UserViewModel>.NotFound("User not found");
            return Result<UserViewModel>.SuccessWithData(_mapper.Map<UserViewModel>(user));
        }

        public async Task<Result<List<UserShortViewModel>>> SearchUsersAsync(SearchUserOptions searchUserOptions)
        {
            searchUserOptions.PrepareOptions();

            IQueryable<User> query = _db.Users;

            query = query.AsNoTracking();

            query = query.Skip(searchUserOptions.Offset).Take(searchUserOptions.Count);

            if (!string.IsNullOrEmpty(searchUserOptions.FirstName))
                query = query.Where(x => x.FirstName.Contains(searchUserOptions.FirstName));

            if (!string.IsNullOrEmpty(searchUserOptions.LastName))
                query = query.Where(x => x.LastName.Contains(searchUserOptions.LastName));

            //Other filters


            var result = await query.OrderBy(x => x.Id).ToListAsync();

            return Result<List<UserShortViewModel>>.SuccessWithData(_mapper.Map<List<UserShortViewModel>>(result));
        }
    }
}
