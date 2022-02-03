using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DUT.Application.Services.Implementations
{
    public class BaseService<T> where T : class
    {
        private readonly DUTDbContext _db;
        protected IEnumerable<T> Exists { get; set; }
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
            if (items != null && items.Count > 0)
            {
                Exists = items;
                return true;
            }
            return false;
        }
    }
}
