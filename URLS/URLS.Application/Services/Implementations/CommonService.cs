using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using URLS.Application.Services.Interfaces;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class CommonService : ICommonService
    {
        private readonly URLSDbContext _db;
        public CommonService(URLSDbContext db)
        {
            _db = db;
        }

        public async Task<int> CountAsync<TResult>(Expression<Func<TResult, bool>> expression = null) where TResult : class
        {
            if (expression == null)
                return await _db.Set<TResult>().CountAsync();
            return await _db.Set<TResult>().CountAsync(expression);
        }

        public async Task<bool> IsExistAsync<TResult>(Expression<Func<TResult, bool>> expression = null) where TResult : class
        {
            if(expression == null)
                return await _db.Set<TResult>().AnyAsync();
            return await _db.Set<TResult>().AnyAsync(expression);
        }

        public async Task<(bool, List<TResult>)> IsExistWithResultsAsync<TResult>(Expression<Func<TResult, bool>> expression = null) where TResult : class
        {
            if (expression == null)
            {
                var allItems = await _db.Set<TResult>().AsNoTracking().ToListAsync();
                return (allItems.Count > 0, allItems);
            }
            var items = await _db.Set<TResult>().AsNoTracking().Where(expression).ToListAsync();
            return (items.Count > 0, items);
        }
    }
}