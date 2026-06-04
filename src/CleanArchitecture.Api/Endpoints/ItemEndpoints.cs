using CleanArchitecture.Api.Extensions;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Endpoints;

/// <summary>
/// Minimal API endpoints for Item management
/// Following REST principles and clean architecture
/// </summary>
public static class ItemEndpoints
{
    public static IEndpointRouteBuilder MapItemEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/items")
            .WithTags("Items");

        // GET /api/items
        group.MapGet("/", GetAllItems)
            .WithName("GetAllItems")
            .WithSummary("Get all items")
            .Produces<IEnumerable<ItemResponse>>(StatusCodes.Status200OK);

        // GET /api/items/{id}
        group.MapGet("/{id:guid}", GetItemById)
            .WithName("GetItemById")
            .WithSummary("Get item by ID")
            .Produces<ItemResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        // POST /api/items
        group.MapPost("/", CreateItem)
            .WithName("CreateItem")
            .WithSummary("Create a new item")
            .Produces<ItemResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        // PUT /api/items/{id}
        group.MapPut("/{id:guid}", UpdateItem)
            .WithName("UpdateItem")
            .WithSummary("Update an existing item")
            .Produces<ItemResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        // DELETE /api/items/{id}
        group.MapDelete("/{id:guid}", DeleteItem)
            .WithName("DeleteItem")
            .WithSummary("Delete an item")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> GetAllItems(
        IItemService itemService,
        CancellationToken cancellationToken)
    {
        var result = await itemService.GetAllItemsAsync(cancellationToken).ConfigureAwait(true);
        return result.Match(
            items => Results.Ok(items),
            errors => Results.Problem(errors.ToProblemDetails()));
    }

    private static async Task<IResult> GetItemById(
        Guid id,
        IItemService itemService,
        CancellationToken cancellationToken)
    {
        var result = await itemService.GetItemByIdAsync(id, cancellationToken).ConfigureAwait(true);
        return result.Match(
            item => Results.Ok(item),
            errors => Results.Problem(errors.ToProblemDetails()));
    }

    private static async Task<IResult> CreateItem(
        CreateItemRequest request,
        IItemService itemService,
        CancellationToken cancellationToken)
    {
        var result = await itemService.CreateItemAsync(request, cancellationToken).ConfigureAwait(true);
        return result.Match(
            item => Results.CreatedAtRoute("GetItemById", new { id = item.Id }, item),
            errors => Results.Problem(errors.ToProblemDetails()));
    }

    private static async Task<IResult> UpdateItem(
        Guid id,
        UpdateItemRequest request,
        IItemService itemService,
        CancellationToken cancellationToken)
    {
        var result = await itemService.UpdateItemAsync(id, request, cancellationToken).ConfigureAwait(true);
        return result.Match(
            item => Results.Ok(item),
            errors => Results.Problem(errors.ToProblemDetails()));
    }

    private static async Task<IResult> DeleteItem(
        Guid id,
        IItemService itemService,
        CancellationToken cancellationToken)
    {
        var result = await itemService.DeleteItemAsync(id, cancellationToken).ConfigureAwait(true);
        return result.Match(
            _ => Results.NoContent(),
            errors => Results.Problem(errors.ToProblemDetails()));
    }
}
