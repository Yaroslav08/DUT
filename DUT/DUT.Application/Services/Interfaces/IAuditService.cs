using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Audit;
namespace DUT.Application.Services.Interfaces
{
    public interface IAuditService
    {
        Task<Result<AuditViewModel<T>>> GetAuditByIdAsync<T>(long id);
        Task<Result<List<AuditViewModel<T>>>> GetAuditsByItemIdAsync<T>(string id, string entity);
        Task<Result<bool>> CreateAuditAsync(AuditCreateModel audit);
    }
}