using System.Linq.Expressions;

namespace DUT.Application.Services.Interfaces
{
    public interface IBaseService<T> where T : class
    {
        public IEnumerable<T> Exists { get; set; }
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
        Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate = null);
    }
}
