using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;

namespace ProductApi.Infrastructure.Data.Repositories;

public class UnitOfWork : Application.Interfaces.IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private Application.Interfaces.IProductRepository? _products;
    private Application.Interfaces.IRepository<Item>? _items;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Application.Interfaces.IProductRepository Products =>
        _products ??= new ProductRepository(_dbContext);

    public Application.Interfaces.IRepository<Item> Items =>
        _items ??= new Repository<Item>(_dbContext);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.RollbackTransactionAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
