using ErrorOr;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Errors;

namespace CleanArchitecture.Domain.Entities;

/// <summary>
/// Item aggregate root - represents a manageable item with name and description
/// </summary>
public sealed class Item : Entity
{
    private string _name = string.Empty;
    private string _description = string.Empty;

    public string Name => _name;
    public string Description => _description;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Required by EF Core
    private Item()
    {
        CreatedAt = DateTime.UtcNow;
    }

    private Item(string name, string description) : base()
    {
        _name = name;
        _description = description;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method to create a new Item with validation
    /// </summary>
    public static ErrorOr<Item> Create(string name, string description)
    {
        var validationResult = ValidateItem(name, description);
        if (validationResult.IsError)
            return validationResult.Errors;

        return new Item(name, description);
    }

    /// <summary>
    /// Updates the item's properties with validation
    /// </summary>
    public ErrorOr<Updated> Update(string name, string description)
    {
        var validationResult = ValidateItem(name, description);
        if (validationResult.IsError)
            return validationResult.Errors;

        _name = name;
        _description = description;
        UpdatedAt = DateTime.UtcNow;

        return Result.Updated;
    }

    private static ErrorOr<Success> ValidateItem(string name, string description)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add(ItemErrors.NameEmpty);
        else if (name.Length > 200)
            errors.Add(ItemErrors.NameTooLong);

        if (description?.Length > 1000)
            errors.Add(ItemErrors.DescriptionTooLong);

        return errors.Count > 0 ? errors : Result.Success;
    }
}
