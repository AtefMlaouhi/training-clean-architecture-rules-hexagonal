using Bogus;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Errors;
using CleanArchitecture.Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace CleanArchitecture.Application.Tests.Services;

public class ItemServiceTests
{
    private readonly IItemRepository _mockRepository;
    private readonly ItemService _sut;
    private readonly Faker _faker;

    public ItemServiceTests()
    {
        _mockRepository = Substitute.For<IItemRepository>();
        _sut = new ItemService(_mockRepository);
        _faker = new Faker();
    }

    #region GetItemByIdAsync Tests

    [Fact]
    public async Task GetItemByIdAsync_WhenItemExists_ShouldReturnItemResponse()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        typeof(Item).GetProperty("Id")!.SetValue(item, itemId);

        _mockRepository.GetByIdAsync(itemId, Arg.Any<CancellationToken>())
            .Returns(item);

        // Act
        var result = await _sut.GetItemByIdAsync(itemId);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Id.Should().Be(itemId);
        result.Value.Name.Should().Be(item.Name);
        result.Value.Description.Should().Be(item.Description);
        result.Value.CreatedAt.Should().Be(item.CreatedAt);
        result.Value.UpdatedAt.Should().Be(item.UpdatedAt);
    }

    [Fact]
    public async Task GetItemByIdAsync_WhenItemDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockRepository.GetByIdAsync(itemId, Arg.Any<CancellationToken>())
            .Returns((Item?)null);

        // Act
        var result = await _sut.GetItemByIdAsync(itemId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ItemErrors.NotFound(itemId));
    }

    [Fact]
    public async Task GetItemByIdAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        // Act
        await _sut.GetItemByIdAsync(itemId, cancellationToken);

        // Assert
        await _mockRepository.Received(1)
            .GetByIdAsync(itemId, cancellationToken);
    }

    #endregion

    #region GetAllItemsAsync Tests

    [Fact]
    public async Task GetAllItemsAsync_WhenItemsExist_ShouldReturnAllItems()
    {
        // Arrange
        var items = new List<Item>
        {
            Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value,
            Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value,
            Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value
        };

        _mockRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(items);

        // Act
        var result = await _sut.GetAllItemsAsync();

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().HaveCount(3);
        result.Value.Should().AllSatisfy(response =>
        {
            response.Id.Should().NotBeEmpty();
            response.Name.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public async Task GetAllItemsAsync_WhenNoItems_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Item>());

        // Act
        var result = await _sut.GetAllItemsAsync();

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllItemsAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        // Act
        await _sut.GetAllItemsAsync(cancellationToken);

        // Assert
        await _mockRepository.Received(1)
            .GetAllAsync(cancellationToken);
    }

    #endregion

    #region CreateItemAsync Tests

    [Fact]
    public async Task CreateItemAsync_WithValidRequest_ShouldCreateAndReturnItem()
    {
        // Arrange
        var request = new CreateItemRequest(
            _faker.Commerce.ProductName(),
            _faker.Lorem.Sentence());

        _mockRepository.AddAsync(Arg.Any<Item>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Item>());

        // Act
        var result = await _sut.CreateItemAsync(request);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be(request.Name);
        result.Value.Description.Should().Be(request.Description);
        result.Value.Id.Should().NotBeEmpty();

        await _mockRepository.Received(1)
            .AddAsync(Arg.Is<Item>(i => i.Name == request.Name), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateItemAsync_WithEmptyName_ShouldReturnValidationError()
    {
        // Arrange
        var request = new CreateItemRequest(string.Empty, _faker.Lorem.Sentence());

        // Act
        var result = await _sut.CreateItemAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ItemErrors.NameEmpty);

        await _mockRepository.DidNotReceive()
            .AddAsync(Arg.Any<Item>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateItemAsync_WithNameTooLong_ShouldReturnValidationError()
    {
        // Arrange
        var request = new CreateItemRequest(
            _faker.Random.String2(201),
            _faker.Lorem.Sentence());

        // Act
        var result = await _sut.CreateItemAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ItemErrors.NameTooLong);

        await _mockRepository.DidNotReceive()
            .AddAsync(Arg.Any<Item>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateItemAsync_WithDescriptionTooLong_ShouldReturnValidationError()
    {
        // Arrange
        var request = new CreateItemRequest(
            _faker.Commerce.ProductName(),
            _faker.Random.String2(1001));

        // Act
        var result = await _sut.CreateItemAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ItemErrors.DescriptionTooLong);

        await _mockRepository.DidNotReceive()
            .AddAsync(Arg.Any<Item>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateItemAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var request = new CreateItemRequest(
            _faker.Commerce.ProductName(),
            _faker.Lorem.Sentence());
        var cancellationToken = new CancellationToken();

        _mockRepository.AddAsync(Arg.Any<Item>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Item>());

        // Act
        await _sut.CreateItemAsync(request, cancellationToken);

        // Assert
        await _mockRepository.Received(1)
            .AddAsync(Arg.Any<Item>(), cancellationToken);
    }

    #endregion

    #region UpdateItemAsync Tests

    [Fact]
    public async Task UpdateItemAsync_WithValidRequest_ShouldUpdateAndReturnItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        typeof(Item).GetProperty("Id")!.SetValue(existingItem, itemId);

        var request = new UpdateItemRequest(
            _faker.Commerce.ProductName(),
            _faker.Lorem.Sentence());

        _mockRepository.GetByIdAsync(itemId, Arg.Any<CancellationToken>())
            .Returns(existingItem);

        // Act
        var result = await _sut.UpdateItemAsync(itemId, request);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Id.Should().Be(itemId);
        result.Value.Name.Should().Be(request.Name);
        result.Value.Description.Should().Be(request.Description);
        result.Value.UpdatedAt.Should().NotBeNull();

        await _mockRepository.Received(1)
            .UpdateAsync(existingItem, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateItemAsync_WhenItemDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var request = new UpdateItemRequest(
            _faker.Commerce.ProductName(),
            _faker.Lorem.Sentence());

        _mockRepository.GetByIdAsync(itemId, Arg.Any<CancellationToken>())
            .Returns((Item?)null);

        // Act
        var result = await _sut.UpdateItemAsync(itemId, request);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ItemErrors.NotFound(itemId));

        await _mockRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Item>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateItemAsync_WithEmptyName_ShouldReturnValidationError()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        typeof(Item).GetProperty("Id")!.SetValue(existingItem, itemId);

        var request = new UpdateItemRequest(string.Empty, _faker.Lorem.Sentence());

        _mockRepository.GetByIdAsync(itemId, Arg.Any<CancellationToken>())
            .Returns(existingItem);

        // Act
        var result = await _sut.UpdateItemAsync(itemId, request);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ItemErrors.NameEmpty);

        await _mockRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<Item>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateItemAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        typeof(Item).GetProperty("Id")!.SetValue(existingItem, itemId);

        var request = new UpdateItemRequest(
            _faker.Commerce.ProductName(),
            _faker.Lorem.Sentence());
        var cancellationToken = new CancellationToken();

        _mockRepository.GetByIdAsync(itemId, Arg.Any<CancellationToken>())
            .Returns(existingItem);

        // Act
        await _sut.UpdateItemAsync(itemId, request, cancellationToken);

        // Assert
        await _mockRepository.Received(1)
            .GetByIdAsync(itemId, cancellationToken);
        await _mockRepository.Received(1)
            .UpdateAsync(existingItem, cancellationToken);
    }

    #endregion

    #region DeleteItemAsync Tests

    [Fact]
    public async Task DeleteItemAsync_WhenItemExists_ShouldDeleteAndReturnSuccess()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockRepository.ExistsAsync(itemId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _sut.DeleteItemAsync(itemId);

        // Assert
        result.IsError.Should().BeFalse();

        await _mockRepository.Received(1)
            .DeleteAsync(itemId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteItemAsync_WhenItemDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockRepository.ExistsAsync(itemId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _sut.DeleteItemAsync(itemId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(ItemErrors.NotFound(itemId));

        await _mockRepository.DidNotReceive()
            .DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteItemAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        _mockRepository.ExistsAsync(itemId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        await _sut.DeleteItemAsync(itemId, cancellationToken);

        // Assert
        await _mockRepository.Received(1)
            .ExistsAsync(itemId, cancellationToken);
        await _mockRepository.Received(1)
            .DeleteAsync(itemId, cancellationToken);
    }

    #endregion
}
