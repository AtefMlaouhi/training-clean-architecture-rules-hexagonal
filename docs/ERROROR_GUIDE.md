# ErrorOr Integration Guide

## Overview

This project now uses the **ErrorOr** library for functional error handling instead of exceptions and null returns. This provides:
- Type-safe error handling
- Railway-oriented programming
- Better error composition
- Reduced exception overhead

## Architecture Changes

### 1. Primary Constructors (C# 12)

All classes now use primary constructors for dependency injection:

```csharp
// Before
public sealed class ItemService : IItemService
{
    private readonly IItemRepository _repository;
    
    public ItemService(IItemRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
}

// After
public sealed class ItemService(IItemRepository repository) : IItemService
{
    // repository is automatically available as a parameter
}
```

### 2. ErrorOr Return Types

All service methods now return `ErrorOr<T>` instead of nullable types or throwing exceptions:

```csharp
// Before
Task<ItemDto?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default);

// After
Task<ErrorOr<ItemResponse>> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
```

### 3. Request/Response Models

DTOs have been renamed to follow Request/Response pattern:

- `CreateItemDto` → `CreateItemRequest`
- `UpdateItemDto` → `UpdateItemRequest`
- `ItemDto` → `ItemResponse`

## Error Handling Examples

### Domain Layer

```csharp
public static ErrorOr<Item> Create(string name, string description)
{
    var validationResult = ValidateItem(name, description);
    if (validationResult.IsError)
        return validationResult.Errors;

    return new Item(name, description);
}
```

### Application Layer

```csharp
public async Task<ErrorOr<ItemResponse>> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
{
    var item = await repository.GetByIdAsync(id, cancellationToken);
    
    return item is not null 
        ? MapToResponse(item) 
        : ItemErrors.NotFound(id);
}
```

### API Layer

```csharp
private static async Task<IResult> GetItemById(
    Guid id,
    IItemService itemService,
    CancellationToken cancellationToken)
{
    var result = await itemService.GetItemByIdAsync(id, cancellationToken);
    return result.Match(
        item => Results.Ok(item),
        errors => Results.Problem(errors.ToProblemDetails()));
}
```

## Error Types

The project defines domain-specific errors in `ItemErrors.cs`:

```csharp
public static class ItemErrors
{
    public static Error NotFound(Guid id) => Error.NotFound(
        "Item.NotFound",
        $"Item with ID '{id}' was not found");

    public static Error NameEmpty => Error.Validation(
        "Item.NameEmpty",
        "Item name cannot be empty");

    public static Error NameTooLong => Error.Validation(
        "Item.NameTooLong",
        "Item name cannot exceed 200 characters");
}
```

## API Response Examples

### Successful Response

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Sample Item",
  "description": "This is a sample",
  "createdAt": "2026-06-02T10:30:00Z",
  "updatedAt": null
}
```

### Error Response (Validation)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": "Item name cannot be empty",
  "errors": [
    "Item name cannot be empty"
  ]
}
```

### Error Response (Not Found)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Item with ID '3fa85f64-5717-4562-b3fc-2c963f66afa6' was not found"
}
```

## Testing ErrorOr Endpoints

### Test Validation Error

```bash
curl -X POST http://localhost:5214/api/items \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"\",\"description\":\"Test\"}"
```

Expected Response:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": "Item name cannot be empty"
}
```

### Test Not Found Error

```bash
curl http://localhost:5214/api/items/00000000-0000-0000-0000-000000000000
```

Expected Response:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Item with ID '00000000-0000-0000-0000-000000000000' was not found"
}
```

### Test Successful Creation

```bash
curl -X POST http://localhost:5214/api/items \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"Valid Item\",\"description\":\"This is valid\"}"
```

Expected Response:
```json
{
  "id": "...",
  "name": "Valid Item",
  "description": "This is valid",
  "createdAt": "2026-06-02T...",
  "updatedAt": null
}
```

## Benefits of ErrorOr

1. **Type Safety**: Errors are part of the type signature
2. **No Exceptions**: Better performance and control flow
3. **Composability**: Easy to chain operations
4. **Railway Pattern**: Success/failure paths are clear
5. **Standardization**: Consistent error handling across all layers

## Migration Summary

| Before | After |
|--------|-------|
| `throw new ArgumentException()` | `return ItemErrors.NameEmpty` |
| `return null` | `return ItemErrors.NotFound(id)` |
| `ItemDto` | `ItemResponse` |
| `CreateItemDto` | `CreateItemRequest` |
| Traditional constructors | Primary constructors |
| Try-catch blocks | Match expressions |

## Resources

- [ErrorOr GitHub](https://github.com/amantinband/error-or)
- [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/)
- [C# 12 Primary Constructors](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12#primary-constructors)
