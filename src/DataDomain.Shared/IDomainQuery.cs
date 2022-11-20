using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain.Shared
{
    public interface IQueryBuilder<TEntity>
    {
        IEnumerable<TEntity> BuildQuery(IQueryable<TEntity> entity);
    }

    public interface IDomainQuery<TEntity>
    {
        Task<IEnumerable<TEntity>> ExecuteQueryAsync(IQueryBuilder<TEntity>? queryBuilder = null);
        Task<IEnumerable<TEntity>> ExecuteQueryAsync(Func<IQueryable<TEntity>, IEnumerable<TEntity>> queryFunction);
    }
}
