using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DUT.Application.Services.Implementations
{
    public class BaseService<T> where T : class
    {
        private readonly DUTDbContext _db;
        public BaseService(DUTDbContext db)
        {
            _db = db;
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return predicate is null ?
                await _db.Set<T>().CountAsync() :
                await _db.Set<T>().CountAsync(predicate);
        }

        public async Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate = null)
        {
            var items = await _db.Set<T>().AsNoTracking().Where(predicate).ToListAsync();
            return items.Any();
        }
    }
}
