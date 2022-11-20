using CQRS.Events.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataDomain.EFCore
{
    public class DbContextEntityBuilder<TDbContext> : IEntityBuilder where TDbContext : DbContext
    {
        private readonly DbContext dbContext;

        public DbContextEntityBuilder(TDbContext productDbContext)
        {
            this.dbContext = productDbContext;
        }

        public IQueryable<TEntityType> GetQueryable<TEntityType>() where TEntityType : class
        {
            return this.dbContext.Set<TEntityType>();
        }

        public IQueryable<TCollectionType> LoadCollection<TEntityType, TCollectionType>(TEntityType entity, Expression<Func<TEntityType, IEnumerable<TCollectionType>>> loadCollectionFunction) where TEntityType : class where TCollectionType : class
        {
            return this.dbContext
                .Entry<TEntityType>(entity)
                .Collection(loadCollectionFunction)
                .Query();
        }

        public TEntityType CreateEntity<TEntityType>(TEntityType? entityType = null) where TEntityType : class, new()
        {
            var entity = entityType ?? new TEntityType();
            this.dbContext.Add<TEntityType>(entity);

            return entity;
        }

        public void RemoveEntity<TEntityType>(TEntityType entityType) where TEntityType : class
        {
            this.dbContext.Remove<TEntityType>(entityType);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await this.dbContext.SaveChangesAsync(cancellationToken);
        }

        public void UpdateEntity<TEntityType>(TEntityType entityType) where TEntityType : class
        {
            this.dbContext.Update<TEntityType>(entityType);
        }
    }
}
