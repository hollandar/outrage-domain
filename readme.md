# Outrage Event Sourcing and DDD for EFCore

This repo integrates with eventsourcedb or other event databases, and provides CQRS for .NET applications via two interfaces.
Supports clean architecture by separating storage from the domain.

## Domain objects

Domain objects should be defined in a Domain class library along with builders and events, domain objects are organised as aggregate roots (IAggregateRoot).

```C#
public class Product : IAggregateRoot, IEventSourcing
{
    public Guid Id { get; set; } = new();
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int QuantityOnHand { get; set; }
    public ulong Version { get; set; }
    public virtual ICollection<ProductTag> Tags { get; set; }

    public override string ToString()
    {
        return $"Sku: {Sku}, Name: {Name}, QuantityOnHand: {QuantityOnHand}.";
    }
}

public class ProductTag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public String Tag { get; set; } = String.Empty;
    public Guid? ProductId { get; set; }
    public virtual Product? Product { get; set; }
}
```

Product Tag is a domain object that is within the Product aggregate root and is managed by product events.

## Events 

Events are actions (usually records of data) that can be applied as an instruction to the domain model, for example:

```C#
public record CreateProduct : IEvent
    {
        public Guid Id { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
    }
```

## Builders

Builders are the command factory that execute events against the aggregate root.

```C#
public class ProductBuilder : AggregateRootBuilder<Product>
{
    public ProductBuilder(
        IEntityBuilder entityBuilder) : base(entityBuilder)
    {
    }

    protected override async Task ApplyEventAsync(Product? product, IEvent eventRecord)
    {
        switch (eventRecord)
        {
            case CreateProduct createProduct:
                await CreateProduct(createProduct);
                break;
            default:
                throw new Exception($"Unknown change type {eventRecord.GetType()}.");
        }
    }


    private Task CreateProduct(CreateProduct createProduct)
    {
        var product = EntityBuilder.CreateEntity<Product>();
        product.Id = createProduct.Id;
        product.Sku = createProduct.Sku;
        product.Name = createProduct.Name;
        product.QuantityOnHand = 0;

        return Task.CompletedTask;
    }
}
```

IEntityBuilder is provided by the EFCore infrastructure and is used to perform a number of EF functions, it is injected such that the Entity Framework provider can be replaced with any other ORM.  It provides the following functions:

* GetQueryable<TEntityType>() - return a queryable for a specific entity.
* LoadCollection<TEntityType, TCollectionType(TEntityType, Expression<Func<TEntityType, IEnumerable<TCollectionType>>>) -  lazy load a collection for an already loaded entity
* CreateEntity<TEntityType>(TEntityType? = null) - Create an entity (tracking it in ef) or add an already created entity (tracking it in ef).
* RemoveEntity<TEntityType>(TEntityType) - Remove an entity from ef.
* UpdateEntity<TEntityType>(TEntityType) - Begin tracking an entity in ef.
* SaveChangesAsync(CancellationToken) - Save ef changes.

## Aggregate root relationship commands - adding a ProductTag

For commands that update objects related to the aggregate root, use the IEntityBuilder to traverse the aggregate root, for example a command that adds a tag to the aggregate root using a ProductTag entity: 

```c#
public record TagProduct : IEvent
{
    public Guid Id { get; set; }
    public string Tag { get; set; }
}

public record UntagProduct : IEvent
{
    public Guid Id { get; set; }
    public string Tag { get; set; }
}
```

TagProduct creates a ProductTag and associates it with the product.
UntagProduct lazily loads the collection of tags, find the one being changed and removes it.
```c#
private Task TagProduct(Product product, TagProduct tagProduct)
{
    Debug.Assert(product != null);

    var productTag = EntityBuilder.CreateEntity<ProductTag>();
    productTag.ProductId = tagProduct.Id;
    productTag.Tag = tagProduct.Tag;

    return Task.CompletedTask;
}

private Task UntagProduct(Product product, UntagProduct untagProduct)
    {
        Debug.Assert(product != null);

        var productTags = this.EntityBuilder.LoadCollection(product, r => r.Tags);
        var tags = productTags.Where(r => r.Tag == untagProduct.Tag);
        if (tags?.Any() ?? false)
        {
            foreach (var tag in tags)
                this.EntityBuilder.RemoveEntity(tag);
        }

        return Task.CompletedTask;
    }
```


# Event Sourcing

Writing events to event sourcing is automatic and occurs after they are successfully applied to the domain.
Events will be written for aggregate roots only where they implement the marker interface IEventSourcing.

# Setup

For event sourcing using EventStoreDB, add the following to dependency services.  EventStoreDB is a setting in appsettings containing the EventStoreDB connection string:
```c#
builder.Services.AddOptions<EventStoreDBOptions>().BindConfiguration("EventStoreDB");
builder.Services.AddScoped<IEventStorageService, EventStoreDBStorageService>();
```

For domain and events, add the entity builder for your DbContext, and then define a domain for each aggregate root:
```c#
builder.Services.AddEntityBuilder<ProductDbContext>();
builder.Services.AddDomainObject<Product, ProductBuilder>();
```

# Executing Commands

Once you have a command, you can apply it to the domain model by injecting IDomainCommand<Entity> and calling ApplyAsync, passing the aggregate root ID and the command:
```c#
app.MapPost("/product", async ([FromBody] CreateProduct createProduct, IDomainCommand<ShoppingCart.Domain.Product> productDomain) => {
    await productDomain.ApplyAsync(createProduct.Id, createProduct);

    return Results.Ok();
});
```

# Executing Queries

To execute a query, define a query builder.  A query builder allows you to filter the aggregate root collection.  Operations such as EF Include() can also be used, but be aware that they tie your implementation to entity framework core.

```c#
internal class ProductsByNameQueryBuilder : IQueryBuilder<Product>
{
    public IEnumerable<Product> BuildQuery(IQueryable<Product> entity)
    {
        return entity.OrderBy(r => r.Name);
    }
}
```

You can then execute the query against the domain and return the results.  You can of course project the aggregate object you receive into whatever form you require:
```c#
app.MapGet("/product", async (IDomainQuery<Product> productDomain) =>
{
    var products = await productDomain.ExecuteQueryAsync(new ProductsByNameQueryBuilder());

    return Results.Ok(products);
});
```