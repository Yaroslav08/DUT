using URLS.Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace URLS.Application.Services.Implementations
{
    public class CommonService : ICommonService
    {
        private readonly DbContext _db;
        public CommonService(DbContext db)
        {
            _db = db;
        }

        public async Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class
        {
            if (predicate == null)
                return await _db.Set<T>().CountAsync();
            return await _db.Set<T>().CountAsync(predicate);
        }

        public async Task<bool> IsExistAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class
        {
            if (predicate == null)
                return await _db.Set<T>().AnyAsync();
            return await _db.Set<T>().AnyAsync(predicate);
        }
    }
}