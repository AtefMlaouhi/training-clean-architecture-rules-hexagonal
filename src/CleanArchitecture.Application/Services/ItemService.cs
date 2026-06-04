using ErrorOr;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Errors;
using CleanArchitecture.Domain.Repositories;

namespace CleanArchitecture.Application.Services;

/// <summary>
/// Implementation of Item management use cases
/// Contains application business logic
/// </summary>
public sealed class ItemService(IItemRepository repository) : IItemService
{
    public async Task<ErrorOr<ItemResponse>> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await repository.GetByIdAsync(id, cancellationToken);

        return item is not null
            ? MapToResponse(item)
            : ItemErrors.NotFound(id);
    }

    public async Task<ErrorOr<IEnumerable<ItemResponse>>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        var items = await repository.GetAllAsync(cancellationToken);
        return items.Select(MapToResponse).ToList();
    }

    public async Task<ErrorOr<ItemResponse>> CreateItemAsync(CreateItemRequest request, CancellationToken cancellationToken = default)
    {
        var itemResult = Item.Create(request.Name, request.Description ?? string.Empty);

        if (itemResult.IsError)
            return itemResult.Errors;

        var createdItem = await repository.AddAsync(itemResult.Value, cancellationToken);
        return MapToResponse(createdItem);
    }

    public async Task<ErrorOr<ItemResponse>> UpdateItemAsync(Guid id, UpdateItemRequest request, CancellationToken cancellationToken = default)
    {
        var item = await repository.GetByIdAsync(id, cancellationToken);

        if (item is null)
            return ItemErrors.NotFound(id);

        var updateResult = item.Update(request.Name, request.Description ?? string.Empty);

        if (updateResult.IsError)
            return updateResult.Errors;

        await repository.UpdateAsync(item, cancellationToken);
        return MapToResponse(item);
    }

    public async Task<ErrorOr<Deleted>> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var exists = await repository.ExistsAsync(id, cancellationToken);

        if (!exists)
            return ItemErrors.NotFound(id);

        await repository.DeleteAsync(id, cancellationToken);
        return Result.Deleted;
    }

    private static ItemResponse MapToResponse(Item item) => new(
        item.Id,
        item.Name,
        item.Description,
        item.CreatedAt,
        item.UpdatedAt
    );
}
