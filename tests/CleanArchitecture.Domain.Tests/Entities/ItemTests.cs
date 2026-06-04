using Bogus;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Errors;
using FluentAssertions;

namespace CleanArchitecture.Domain.Tests.Entities;

public class ItemTests
{
    private readonly Faker _faker;

    public ItemTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void Create_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var description = _faker.Lorem.Sentence();

        // Act
        var result = Item.Create(name, description);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be(name);
        result.Value.Description.Should().Be(description);
        result.Value.Id.Should().NotBeEmpty();
        result.Value.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.Value.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyName_ShouldReturnNameEmptyError()
    {
        // Arrange
        var name = string.Empty;
        var description = _faker.Lorem.Sentence();

        // Act
        var result = Item.Create(name, description);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.FirstError.Should().Be(ItemErrors.NameEmpty);
    }

    [Fact]
    public void Create_WithWhitespaceName_ShouldReturnNameEmptyError()
    {
        // Arrange
        var name = "   ";
        var description = _faker.Lorem.Sentence();

        // Act
        var result = Item.Create(name, description);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.FirstError.Should().Be(ItemErrors.NameEmpty);
    }

    [Fact]
    public void Create_WithNameTooLong_ShouldReturnNameTooLongError()
    {
        // Arrange
        var name = _faker.Random.String2(201);
        var description = _faker.Lorem.Sentence();

        // Act
        var result = Item.Create(name, description);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.FirstError.Should().Be(ItemErrors.NameTooLong);
    }

    [Fact]
    public void Create_WithDescriptionTooLong_ShouldReturnDescriptionTooLongError()
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var description = _faker.Random.String2(1001);

        // Act
        var result = Item.Create(name, description);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.FirstError.Should().Be(ItemErrors.DescriptionTooLong);
    }

    [Fact]
    public void Create_WithEmptyNameAndDescriptionTooLong_ShouldReturnMultipleErrors()
    {
        // Arrange
        var name = string.Empty;
        var description = _faker.Random.String2(1001);

        // Act
        var result = Item.Create(name, description);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(ItemErrors.NameEmpty);
        result.Errors.Should().Contain(ItemErrors.DescriptionTooLong);
    }

    [Fact]
    public void Create_WithEmptyDescription_ShouldReturnSuccessResult()
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var description = string.Empty;

        // Act
        var result = Item.Create(name, description);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Description.Should().Be(description);
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateItemAndReturnSuccess()
    {
        // Arrange
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        var newName = _faker.Commerce.ProductName();
        var newDescription = _faker.Lorem.Sentence();

        // Act
        var result = item.Update(newName, newDescription);

        // Assert
        result.IsError.Should().BeFalse();
        item.Name.Should().Be(newName);
        item.Description.Should().Be(newDescription);
        item.UpdatedAt.Should().NotBeNull();
        item.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Update_WithEmptyName_ShouldReturnNameEmptyError()
    {
        // Arrange
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        var originalName = item.Name;
        var newDescription = _faker.Lorem.Sentence();

        // Act
        var result = item.Update(string.Empty, newDescription);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ItemErrors.NameEmpty);
        item.Name.Should().Be(originalName);
        item.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Update_WithNameTooLong_ShouldReturnNameTooLongError()
    {
        // Arrange
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        var originalName = item.Name;
        var newName = _faker.Random.String2(201);
        var newDescription = _faker.Lorem.Sentence();

        // Act
        var result = item.Update(newName, newDescription);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ItemErrors.NameTooLong);
        item.Name.Should().Be(originalName);
        item.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Update_WithDescriptionTooLong_ShouldReturnDescriptionTooLongError()
    {
        // Arrange
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        var originalDescription = item.Description;
        var newName = _faker.Commerce.ProductName();
        var newDescription = _faker.Random.String2(1001);

        // Act
        var result = item.Update(newName, newDescription);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ItemErrors.DescriptionTooLong);
        item.Description.Should().Be(originalDescription);
        item.UpdatedAt.Should().BeNull();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(200)]
    public void Create_WithValidNameLength_ShouldReturnSuccess(int nameLength)
    {
        // Arrange
        var name = _faker.Random.String2(nameLength);
        var description = _faker.Lorem.Sentence();

        // Act
        var result = Item.Create(name, description);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Name.Should().HaveLength(nameLength);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(500)]
    [InlineData(1000)]
    public void Create_WithValidDescriptionLength_ShouldReturnSuccess(int descriptionLength)
    {
        // Arrange
        var name = _faker.Commerce.ProductName();
        var description = _faker.Random.String2(descriptionLength);

        // Act
        var result = Item.Create(name, description);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Description.Should().HaveLength(descriptionLength);
    }

    [Fact]
    public void Create_MultipleItems_ShouldHaveUniqueIds()
    {
        // Arrange & Act
        var item1 = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        var item2 = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        var item3 = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;

        // Assert
        item1.Id.Should().NotBe(item2.Id);
        item1.Id.Should().NotBe(item3.Id);
        item2.Id.Should().NotBe(item3.Id);
    }

    [Fact]
    public void Update_MultipleTimes_ShouldUpdateTimestampEachTime()
    {
        // Arrange
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;

        // Act
        item.Update(_faker.Commerce.ProductName(), _faker.Lorem.Sentence());
        var firstUpdate = item.UpdatedAt;

        Thread.Sleep(10); // Small delay to ensure different timestamp

        item.Update(_faker.Commerce.ProductName(), _faker.Lorem.Sentence());
        var secondUpdate = item.UpdatedAt;

        // Assert
        firstUpdate.Should().NotBeNull();
        secondUpdate.Should().NotBeNull();
        secondUpdate.Should().BeAfter(firstUpdate.Value);
    }
}
