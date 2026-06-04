using ErrorOr;
using CleanArchitecture.Application.Models;

namespace CleanArchitecture.Application.Interfaces;

/// <summary>
/// Service interface for Item management use cases
/// Defines the application boundary
/// </summary>
public interface IItemService
{
    Task<ErrorOr<ItemResponse>> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ErrorOr<IEnumerable<ItemResponse>>> GetAllItemsAsync(CancellationToken cancellationToken = default);
    Task<ErrorOr<ItemResponse>> CreateItemAsync(CreateItemRequest request, CancellationToken cancellationToken = default);
    Task<ErrorOr<ItemResponse>> UpdateItemAsync(Guid id, UpdateItemRequest request, CancellationToken cancellationToken = default);
    Task<ErrorOr<Deleted>> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default);
}
