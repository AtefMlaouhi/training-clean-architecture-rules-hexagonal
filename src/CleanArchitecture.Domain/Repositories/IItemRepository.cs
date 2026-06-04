using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Repositories;

/// <summary>
/// Repository interface for Item aggregate
/// Following the repository pattern for data access abstraction
/// </summary>
public interface IItemRepository
{
    Task<Item?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Item>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Item> AddAsync(Item item, CancellationToken cancellationToken = default);
    Task UpdateAsync(Item item, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
