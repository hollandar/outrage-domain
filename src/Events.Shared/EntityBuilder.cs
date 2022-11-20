using System.Linq.Expressions;

namespace CQRS.Events.Shared
{
    public interface IEntityBuilder
    {
        IQueryable<TEntityType> GetQueryable<TEntityType>() where TEntityType : class;
        TEntityType CreateEntity<TEntityType>(TEntityType? entityType = null) where TEntityType : class, new();
        IQueryable<TCollectionType> LoadCollection<TEntityType, TCollectionType>(TEntityType entity, Expression<Func<TEntityType, IEnumerable<TCollectionType>>> loadCollectionFunction) where TEntityType : class where TCollectionType : class;
        void RemoveEntity<TEntityType>(TEntityType entityType) where TEntityType : class;
        void UpdateEntity<TEntityType>(TEntityType entityType) where TEntityType : class;
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
