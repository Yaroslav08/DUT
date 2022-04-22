using URLS.Application.Services.Interfaces;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class PermissionCommentService : IPermissionCommentService
    {
        private readonly URLSDbContext _db;
        private readonly IIdentityService _identityService;
        public PermissionCommentService(URLSDbContext db, IIdentityService identityService)
        {
            _db = db;
            _identityService = identityService;
        }
    }
}