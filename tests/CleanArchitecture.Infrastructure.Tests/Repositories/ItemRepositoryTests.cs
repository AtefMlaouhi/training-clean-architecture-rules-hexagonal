using Bogus;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Database;
using CleanArchitecture.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Tests.Repositories;

public class ItemRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ItemRepository _sut;
    private readonly Faker _faker;

    public ItemRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _sut = new ItemRepository(_context);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenItemExists_ShouldReturnItem()
    {
        // Arrange
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _sut.GetByIdAsync(item.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(item.Id);
        result.Name.Should().Be(item.Name);
        result.Description.Should().Be(item.Description);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldNotTrackEntity()
    {
        // Arrange
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _sut.GetByIdAsync(item.Id);

        // Assert
        _context.Entry(result!).State.Should().Be(EntityState.Detached);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WhenItemsExist_ShouldReturnAllItems()
    {
        // Arrange
        var items = new List<Item>
        {
            Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value,
            Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value,
            Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value
        };

        await _context.Items.AddRangeAsync(items);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyHaveUniqueItems(i => i.Id);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoItems_ShouldReturnEmptyList()
    {
        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnItemsOrderedByCreatedAt()
    {
        // Arrange
        var item1 = Item.Create("Item 1", "Description 1").Value;
        await Task.Delay(10);
        var item2 = Item.Create("Item 2", "Description 2").Value;
        await Task.Delay(10);
        var item3 = Item.Create("Item 3", "Description 3").Value;

        // Add in random order
        await _context.Items.AddAsync(item2);
        await _context.Items.AddAsync(item1);
        await _context.Items.AddAsync(item3);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = (await _sut.GetAllAsync()).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].CreatedAt.Should().BeBefore(result[1].CreatedAt);
        result[1].CreatedAt.Should().BeBefore(result[2].CreatedAt);
    }

    [Fact]
    public async Task GetAllAsync_ShouldNotTrackEntities()
    {
        // Arrange
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().AllSatisfy(i => _context.Entry(i).State.Should().Be(EntityState.Detached));
    }

    #endregion

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_ShouldAddItemToDatabase()
    {
        // Arrange
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;

        // Act
        var result = await _sut.AddAsync(item);

        // Assert
        result.Should().Be(item);
        var savedItem = await _context.Items.FindAsync(item.Id);
        savedItem.Should().NotBeNull();
        savedItem!.Name.Should().Be(item.Name);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnAddedItem()
    {
        // Arrange
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;

        // Act
        var result = await _sut.AddAsync(item);

        // Assert
        result.Id.Should().Be(item.Id);
        result.Name.Should().Be(item.Name);
        result.Description.Should().Be(item.Description);
    }

    [Fact]
    public async Task AddAsync_MultipleItems_ShouldAddAllItems()
    {
        // Arrange
        var item1 = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        var item2 = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;

        // Act
        await _sut.AddAsync(item1);
        await _sut.AddAsync(item2);

        // Assert
        var allItems = await _context.Items.ToListAsync();
        allItems.Should().HaveCount(2);
        allItems.Should().Contain(i => i.Id == item1.Id);
        allItems.Should().Contain(i => i.Id == item2.Id);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ShouldUpdateItem()
    {
        // Arrange
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var newName = _faker.Commerce.ProductName();
        var newDescription = _faker.Lorem.Sentence();
        item.Update(newName, newDescription);

        // Act
        await _sut.UpdateAsync(item);

        // Assert
        var updatedItem = await _context.Items.FindAsync(item.Id);
        updatedItem.Should().NotBeNull();
        updatedItem!.Name.Should().Be(newName);
        updatedItem.Description.Should().Be(newDescription);
        updatedItem.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        // Arrange
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var newName = _faker.Commerce.ProductName();
        item.Update(newName, _faker.Lorem.Sentence());

        // Act
        await _sut.UpdateAsync(item);
        _context.ChangeTracker.Clear();

        // Assert
        var verifyItem = await _context.Items.FindAsync(item.Id);
        verifyItem!.Name.Should().Be(newName);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenItemExists_ShouldRemoveItem()
    {
        // Arrange
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        await _sut.DeleteAsync(item.Id);

        // Assert
        var deletedItem = await _context.Items.FindAsync(item.Id);
        deletedItem.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenItemDoesNotExist_ShouldNotThrow()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var act = async () => await _sut.DeleteAsync(nonExistentId);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteAsync_ShouldPersistDeletion()
    {
        // Arrange
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        await _sut.DeleteAsync(item.Id);
        _context.ChangeTracker.Clear();

        // Assert
        var allItems = await _context.Items.ToListAsync();
        allItems.Should().NotContain(i => i.Id == item.Id);
    }

    #endregion

    #region ExistsAsync Tests

    [Fact]
    public async Task ExistsAsync_WhenItemExists_ShouldReturnTrue()
    {
        // Arrange
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _sut.ExistsAsync(item.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenItemDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.ExistsAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_ShouldNotTrackEntity()
    {
        // Arrange
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        await _sut.ExistsAsync(item.Id);

        // Assert
        _context.ChangeTracker.Entries().Should().BeEmpty();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task FullCrudCycle_ShouldWorkCorrectly()
    {
        // Create
        var item = Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value;
        var addedItem = await _sut.AddAsync(item);
        addedItem.Id.Should().Be(item.Id);
        _context.ChangeTracker.Clear();

        // Read
        var retrievedItem = await _sut.GetByIdAsync(item.Id);
        retrievedItem.Should().NotBeNull();
        retrievedItem!.Name.Should().Be(item.Name);

        // Update
        var newName = _faker.Commerce.ProductName();
        retrievedItem.Update(newName, retrievedItem.Description);
        await _sut.UpdateAsync(retrievedItem);
        _context.ChangeTracker.Clear();

        var updatedItem = await _sut.GetByIdAsync(item.Id);
        updatedItem!.Name.Should().Be(newName);

        // Delete
        await _sut.DeleteAsync(item.Id);
        var deletedItem = await _sut.GetByIdAsync(item.Id);
        deletedItem.Should().BeNull();
    }

    [Fact]
    public async Task ConcurrentOperations_ShouldWorkCorrectly()
    {
        // Arrange
        var items = Enumerable.Range(1, 10)
            .Select(_ => Item.Create(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()).Value)
            .ToList();

        // Act - Add all items concurrently
        await Task.WhenAll(items.Select(item => _sut.AddAsync(item)));

        // Assert
        var allItems = await _sut.GetAllAsync();
        allItems.Should().HaveCount(10);
    }

    #endregion
}
