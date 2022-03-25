using DUT.Application.Extensions;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Audit;
using DUT.Constants.Extensions;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DUT.Application.Services.Implementations
{
    public class AuditService : IAuditService
    {
        private readonly DUTDbContext _db;
        private readonly IIdentityService _identityService;
        public AuditService(DUTDbContext db, IIdentityService identityService)
        {
            _db = db;
            _identityService = identityService;
        }

        public async Task<Result<bool>> CreateAuditAsync(AuditCreateModel model)
        {
            var audit = new Audit
            {
                Entity = model.Entity,
                EntityId = model.EntityId,
                Before = JsonSerializer.Serialize(model.Before),
                After = JsonSerializer.Serialize(model.After)
            };

            audit.PrepareToCreate(_identityService);
            await _db.Audits.AddAsync(audit);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<AuditViewModel<T>>> GetAuditByIdAsync<T>(long id)
        {
            var res = await _db.Audits
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (res == null)
                return Result<AuditViewModel<T>>.NotFound(typeof(Audit).NotFoundMessage(id));

            var auditViewModel = new AuditViewModel<T>
            {
                Id = id,
                Entity = res.Entity,
                Before = JsonSerializer.Deserialize<T>(res.Before),
                After = JsonSerializer.Deserialize<T>(res.After),
                CreatedAt = res.CreatedAt,
                EntityId = res.EntityId,
            };

            return Result<AuditViewModel<T>>.SuccessWithData(auditViewModel);
        }

        public async Task<Result<List<AuditViewModel<T>>>> GetAuditsByItemIdAsync<T>(string id, string entity)
        {
            var items = await _db.Audits
                .AsNoTracking()
                .Where(s => s.EntityId == id && s.Entity == entity)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var audits = items.Select(audit => new AuditViewModel<T>
            {
                Id = audit.Id,
                CreatedAt = audit.CreatedAt,
                Entity = audit.Entity,
                EntityId = audit.EntityId,
                Before = JsonSerializer.Deserialize<T>(audit.Before),
                After = JsonSerializer.Deserialize<T>(audit.After)
            }).ToList();

            return Result<List<AuditViewModel<T>>>.SuccessWithData(audits);
        }
    }
}