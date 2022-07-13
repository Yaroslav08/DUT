using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Identity;
using URLS.Constants;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly URLSDbContext _db;
        public TokenService(URLSDbContext db)
        {
            _db = db;
        }

        public async Task<JwtToken> GetUserTokenAsync(int userId, Guid sessionId, string authType)
        {
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
            return await GetUserTokenAsync(user, sessionId, authType);
        }

        public async Task<JwtToken> GetUserTokenAsync(User user, Guid sessionId, string authType)
        {
            if (user == null)
                return null;

            var currentUserRoles = await _db.UserRoles
                .Where(x => x.UserId == user.Id)
                .Include(x => x.Role)
                .Select(x => x.Role)
                .ToListAsync();

            var claims = new List<System.Security.Claims.Claim>();
            claims.Add(new System.Security.Claims.Claim(CustomClaimTypes.CurrentSessionId, sessionId.ToString()));
            claims.Add(new System.Security.Claims.Claim(CustomClaimTypes.Login, user.Login));
            claims.Add(new System.Security.Claims.Claim(CustomClaimTypes.UserId, user.Id.ToString()));
            claims.Add(new System.Security.Claims.Claim(CustomClaimTypes.UserName, user.UserName));
            claims.Add(new System.Security.Claims.Claim(CustomClaimTypes.FullName, $"{user.LastName} {user.FirstName}"));
            claims.Add(new System.Security.Claims.Claim(CustomClaimTypes.AuthenticationMethod, authType));
            claims.Add(new System.Security.Claims.Claim(CustomClaimTypes.Language, "uk"));

            foreach (var role in currentUserRoles)
            {
                claims.Add(new System.Security.Claims.Claim(CustomClaimTypes.Role, role.Name));
            }

            var userGroups = await _db.UserGroups.AsNoTracking().Where(x => x.UserId == user.Id && x.Status == UserGroupStatus.Member).ToListAsync();

            if (userGroups != null && userGroups.Any())
            {
                foreach (var userGroup in userGroups)
                {
                    claims.Add(new System.Security.Claims.Claim(CustomClaimTypes.GroupMemberId, userGroup.Id.ToString()));
                }
            }


            var roleIds = currentUserRoles.Select(x => x.Id);

            var roleClaims = await _db.RoleClaims.Where(s => roleIds.Contains(s.RoleId)).Include(s => s.Claim).ToListAsync();

            var userRoleClaims = GetUniqClaims(roleClaims);

            if (userRoleClaims != null && userRoleClaims.Count > 0)
            {
                foreach (var roleClaim in userRoleClaims)
                {
                    claims.Add(new System.Security.Claims.Claim(roleClaim.Type, roleClaim.Value));
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
                SessionId = sessionId.ToString()
            };
        }

        private List<Domain.Models.Claim> GetUniqClaims(List<RoleClaim> roleClaims)
        {
            return roleClaims.Select(s => new { Type = s.Claim.Type, Value = s.Claim.Value }).Distinct().Select(x => new Domain.Models.Claim { Type = x.Type, Value = x.Value }).ToList();
        }
    }
}