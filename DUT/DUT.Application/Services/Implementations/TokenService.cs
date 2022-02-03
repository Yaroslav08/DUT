using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Identity;
using DUT.Constants;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DUT.Application.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly DUTDbContext _db;
        public TokenService(DUTDbContext db)
        {
            _db = db;
        }

        public async Task<JwtToken> GetUserTokenAsync(int userId, Guid sessionId, string authType)
        {
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
            return await GetUserTokenAsync(user, sessionId, authType);
        }

        public async Task<JwtToken> GetUserTokenAsync(DUT.Domain.Models.User user, Guid sessionId, string authType)
        {
            if (user == null)
                return null;

            var currentUserRoles = await _db.UserRoles
                .Where(x => x.UserId == user.Id)
                .Include(x => x.Role)
                .Select(x => x.Role)
                .ToListAsync();

            var claims = new List<Claim>();
            claims.Add(new Claim(CustomClaimTypes.CurrentSessionId, sessionId.ToString()));
            claims.Add(new Claim(CustomClaimTypes.Login, user.Login));
            claims.Add(new Claim(CustomClaimTypes.UserId, user.Id.ToString()));
            claims.Add(new Claim(CustomClaimTypes.UserName, user.UserName));
            claims.Add(new Claim(CustomClaimTypes.FullName, $"{user.LastName} {user.FirstName}"));
            claims.Add(new Claim(CustomClaimTypes.AuthenticationMethod, authType));

            foreach (var role in currentUserRoles)
            {
                claims.Add(new Claim(CustomClaimTypes.Role, role.Name));
            }

            var userGroups = await _db.UserGroups.AsNoTracking().Where(x => x.UserId == user.Id && x.Status == UserGroupStatus.Member).ToListAsync();

            if (userGroups != null && userGroups.Any())
            {
                foreach (var userGroup in userGroups)
                {
                    claims.Add(new Claim(CustomClaimTypes.GroupMemberId, userGroup.Id.ToString()));
                }
            }


            var roleIds = currentUserRoles.Select(x => x.Id);

            var permissionClaims = await _db.RoleClaims.Where(s => roleIds.Contains(s.RoleId)).ToListAsync();

            if (permissionClaims != null && permissionClaims.Count > 0)
            {
                permissionClaims = (List<RoleClaim>)permissionClaims.Distinct();
                foreach (var permissionClaim in permissionClaims)
                {
                    claims.Add(new Claim(permissionClaim.Type, permissionClaim.Value));
                }
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            var now = DateTime.Now;
            var expiredAt = now.Add(TimeSpan.FromDays(TokenOptions.LifeTimeInDays));
            var jwt = new JwtSecurityToken(
                    issuer: TokenOptions.Issuer,
                    audience: TokenOptions.Audience,
                    notBefore: now,
                    claims: claimsIdentity.Claims,
                    expires: expiredAt,
                    signingCredentials: new SigningCredentials(TokenOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return new JwtToken
            {
                Token = encodedJwt,
                ExpiredAt = expiredAt,
                Claims = jwt.Claims,
                TokenId = jwt.Id
            };
        }
    }
}