using System.Linq.Expressions;

namespace URLS.Application.Services.Interfaces
{
    public interface ICommonService
    {
        Task<bool> IsExistAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class;
        Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class;
    }
}