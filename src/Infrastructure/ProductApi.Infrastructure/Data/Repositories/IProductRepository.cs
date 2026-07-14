using ProductApi.Domain.Entities;

namespace ProductApi.Infrastructure.Data.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetPagedWithItemsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
