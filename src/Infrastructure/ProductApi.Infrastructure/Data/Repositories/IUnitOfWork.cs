namespace ProductApi.Infrastructure.Data.Repositories;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IRepository<Domain.Entities.Item> Items { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
