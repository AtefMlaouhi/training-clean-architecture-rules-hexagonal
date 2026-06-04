using ErrorOr;

namespace CleanArchitecture.Domain.Errors;

/// <summary>
/// Domain-level errors for Item aggregate
/// </summary>
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

    public static Error DescriptionTooLong => Error.Validation(
        "Item.DescriptionTooLong",
        "Item description cannot exceed 1000 characters");

    public static Error AlreadyExists(string name) => Error.Conflict(
        "Item.AlreadyExists",
        $"Item with name '{name}' already exists");
}
