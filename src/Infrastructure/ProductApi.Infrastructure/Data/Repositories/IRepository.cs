// This interface has moved to Application/Interfaces/IRepository.cs
// Keep this file as a re-export for convenience.
using ProductApi.Application.Interfaces;

namespace ProductApi.Infrastructure.Data.Repositories
{
    public interface IRepository<T> : Application.Interfaces.IRepository<T> where T : class { }
}
