// This interface has moved to Application/Interfaces/IProductRepository.cs
// Keep this file as a re-export for convenience.
using ProductApi.Application.Interfaces;

namespace ProductApi.Infrastructure.Data.Repositories
{
    public interface IProductRepository : Application.Interfaces.IProductRepository { }
}
