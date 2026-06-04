namespace CleanArchitecture.Application.Models;

/// <summary>
/// Request model for creating a new item
/// </summary>
public record CreateItemRequest(string Name, string Description);

/// <summary>
/// Request model for updating an existing item
/// </summary>
public record UpdateItemRequest(string Name, string Description);

/// <summary>
/// Response model for item data
/// </summary>
public record ItemResponse(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
