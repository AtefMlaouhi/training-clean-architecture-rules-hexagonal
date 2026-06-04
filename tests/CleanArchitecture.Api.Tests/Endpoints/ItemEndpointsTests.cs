using System.Net;
using System.Net.Http.Json;
using Bogus;
using CleanArchitecture.Application.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Tests.Endpoints;

public sealed class ItemEndpointsTests : IDisposable
{
    private readonly Faker _faker = new();
    private CustomWebApplicationFactory? _factory;
    private HttpClient? _client;

    private HttpClient CreateClient()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
        return _client;
    }

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    #region GET /api/items

    [Fact]
    public async Task GetAllItemsWhenNoItemsShouldReturnEmptyList()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.GetAsync("/api/items");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadFromJsonAsync<List<ItemResponse>>();
        items.Should().NotBeNull();
        items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllItemsWhenItemsExistShouldReturnAllItems()
    {
        // Arrange
        var client = CreateClient();
        var item1 = new CreateItemRequest(_faker.Commerce.ProductName(), _faker.Lorem.Sentence());
        var item2 = new CreateItemRequest(_faker.Commerce.ProductName(), _faker.Lorem.Sentence());
        await client.PostAsJsonAsync("/api/items", item1);
        await client.PostAsJsonAsync("/api/items", item2);

        // Act
        var response = await client.GetAsync("/api/items");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadFromJsonAsync<List<ItemResponse>>();
        items.Should().NotBeNull();
        items.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    #endregion

    #region GET /api/items/{id}

    [Fact]
    public async Task GetItemByIdWhenItemExistsShouldReturnItem()
    {
        // Arrange
        var client = CreateClient();
        var createRequest = new CreateItemRequest(_faker.Commerce.ProductName(), _faker.Lorem.Sentence());
        var createResponse = await client.PostAsJsonAsync("/api/items", createRequest);
        var createdItem = await createResponse.Content.ReadFromJsonAsync<ItemResponse>();

        // Act
        var response = await client.GetAsync($"/api/items/{createdItem!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var item = await response.Content.ReadFromJsonAsync<ItemResponse>();
        item.Should().NotBeNull();
        item!.Id.Should().Be(createdItem.Id);
        item.Name.Should().Be(createRequest.Name);
        item.Description.Should().Be(createRequest.Description);
    }

    [Fact]
    public async Task GetItemByIdWhenItemDoesNotExistShouldReturnNotFound()
    {
        // Arrange
        var client = CreateClient();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/items/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /api/items

    [Fact]
    public async Task CreateItemWithValidRequestShouldReturnCreated()
    {
        // Arrange
        var client = CreateClient();
        var request = new CreateItemRequest(_faker.Commerce.ProductName(), _faker.Lorem.Sentence());

        // Act
        var response = await client.PostAsJsonAsync("/api/items", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var item = await response.Content.ReadFromJsonAsync<ItemResponse>();
        item.Should().NotBeNull();
        item!.Name.Should().Be(request.Name);
        item.Description.Should().Be(request.Description);
        item.Id.Should().NotBeEmpty();
        item.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain($"/api/items/{item.Id}");
    }

    [Fact]
    public async Task CreateItemWithEmptyNameShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateClient();
        var request = new CreateItemRequest(string.Empty, _faker.Lorem.Sentence());

        // Act
        var response = await client.PostAsJsonAsync("/api/items", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateItemWithNameTooLongShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateClient();
        var request = new CreateItemRequest(_faker.Random.String2(201), _faker.Lorem.Sentence());

        // Act
        var response = await client.PostAsJsonAsync("/api/items", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateItemWithDescriptionTooLongShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateClient();
        var request = new CreateItemRequest(_faker.Commerce.ProductName(), _faker.Random.String2(1001));

        // Act
        var response = await client.PostAsJsonAsync("/api/items", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
    }

    #endregion

    #region PUT /api/items/{id}

    [Fact]
    public async Task UpdateItemWithValidRequestShouldReturnOk()
    {
        // Arrange
        var client = CreateClient();
        var createRequest = new CreateItemRequest(_faker.Commerce.ProductName(), _faker.Lorem.Sentence());
        var createResponse = await client.PostAsJsonAsync("/api/items", createRequest);
        var createdItem = await createResponse.Content.ReadFromJsonAsync<ItemResponse>();

        var updateRequest = new UpdateItemRequest(_faker.Commerce.ProductName(), _faker.Lorem.Sentence());

        // Act
        var response = await client.PutAsJsonAsync($"/api/items/{createdItem!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedItem = await response.Content.ReadFromJsonAsync<ItemResponse>();
        updatedItem.Should().NotBeNull();
        updatedItem!.Id.Should().Be(createdItem.Id);
        updatedItem.Name.Should().Be(updateRequest.Name);
        updatedItem.Description.Should().Be(updateRequest.Description);
        updatedItem.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateItemWhenItemDoesNotExistShouldReturnNotFound()
    {
        // Arrange
        var client = CreateClient();
        var nonExistentId = Guid.NewGuid();
        var updateRequest = new UpdateItemRequest(_faker.Commerce.ProductName(), _faker.Lorem.Sentence());

        // Act
        var response = await client.PutAsJsonAsync($"/api/items/{nonExistentId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateItemWithEmptyNameShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateClient();
        var createRequest = new CreateItemRequest(_faker.Commerce.ProductName(), _faker.Lorem.Sentence());
        var createResponse = await client.PostAsJsonAsync("/api/items", createRequest);
        var createdItem = await createResponse.Content.ReadFromJsonAsync<ItemResponse>();

        var updateRequest = new UpdateItemRequest(string.Empty, _faker.Lorem.Sentence());

        // Act
        var response = await client.PutAsJsonAsync($"/api/items/{createdItem!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateItemWithNameTooLongShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateClient();
        var createRequest = new CreateItemRequest(_faker.Commerce.ProductName(), _faker.Lorem.Sentence());
        var createResponse = await client.PostAsJsonAsync("/api/items", createRequest);
        var createdItem = await createResponse.Content.ReadFromJsonAsync<ItemResponse>();

        var updateRequest = new UpdateItemRequest(_faker.Random.String2(201), _faker.Lorem.Sentence());

        // Act
        var response = await client.PutAsJsonAsync($"/api/items/{createdItem!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region DELETE /api/items/{id}

    [Fact]
    public async Task DeleteItemWhenItemExistsShouldReturnNoContent()
    {
        // Arrange
        var client = CreateClient();
        var createRequest = new CreateItemRequest(_faker.Commerce.ProductName(), _faker.Lorem.Sentence());
        var createResponse = await client.PostAsJsonAsync("/api/items", createRequest);
        var createdItem = await createResponse.Content.ReadFromJsonAsync<ItemResponse>();

        // Act
        var response = await client.DeleteAsync($"/api/items/{createdItem!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify item is deleted
        var getResponse = await client.GetAsync($"/api/items/{createdItem.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteItemWhenItemDoesNotExistShouldReturnNotFound()
    {
        // Arrange
        var client = CreateClient();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/api/items/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Integration Scenarios

    [Fact]
    public async Task FullCrudCycleShouldWorkEndToEnd()
    {
        // Arrange
        var client = CreateClient();

        // Create
        var createRequest = new CreateItemRequest(_faker.Commerce.ProductName(), _faker.Lorem.Sentence());
        var createResponse = await client.PostAsJsonAsync("/api/items", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdItem = await createResponse.Content.ReadFromJsonAsync<ItemResponse>();
        createdItem.Should().NotBeNull();

        // Read
        var getResponse = await client.GetAsync($"/api/items/{createdItem!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedItem = await getResponse.Content.ReadFromJsonAsync<ItemResponse>();
        retrievedItem!.Name.Should().Be(createRequest.Name);

        // Update
        var updateRequest = new UpdateItemRequest(_faker.Commerce.ProductName(), _faker.Lorem.Sentence());
        var updateResponse = await client.PutAsJsonAsync($"/api/items/{createdItem.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedItem = await updateResponse.Content.ReadFromJsonAsync<ItemResponse>();
        updatedItem!.Name.Should().Be(updateRequest.Name);

        // Delete
        var deleteResponse = await client.DeleteAsync($"/api/items/{createdItem.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var verifyResponse = await client.GetAsync($"/api/items/{createdItem.Id}");
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateMultipleItemsShouldAllBeRetrievable()
    {
        // Arrange
        var client = CreateClient();
        var requests = Enumerable.Range(1, 5)
            .Select(_ => new CreateItemRequest(_faker.Commerce.ProductName(), _faker.Lorem.Sentence()))
            .ToList();

        // Act - Create all items
        var createdIds = new List<Guid>();
        foreach (var request in requests)
        {
            var response = await client.PostAsJsonAsync("/api/items", request);
            var item = await response.Content.ReadFromJsonAsync<ItemResponse>();
            createdIds.Add(item!.Id);
        }

        // Assert - Get all items
        var getAllResponse = await client.GetAsync("/api/items");
        var allItems = await getAllResponse.Content.ReadFromJsonAsync<List<ItemResponse>>();
        allItems.Should().NotBeNull();
        allItems!.Count.Should().BeGreaterThanOrEqualTo(5);

        foreach (var id in createdIds)
        {
            allItems.Should().Contain(i => i.Id == id);
        }
    }

    #endregion
}
