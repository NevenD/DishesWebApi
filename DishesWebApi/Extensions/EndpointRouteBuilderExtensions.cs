using DishesWebApi.EndPointFilters;
using DishesWebApi.EndpointHandlers;
using DishesWebApi.Entities;
using System.Net;

namespace DishesWebApi.Extensions
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void RegisterDishesEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var dishesEndPoints = endpoints.MapGroup("/dishes")
                .RequireAuthorization();
            var dishesWithGuidIdEndPoints = dishesEndPoints.MapGroup("/{dishId:guid}");
            var ingridentsEndPoints = dishesEndPoints.MapGroup("/ingredients");

            var dishWithGuidIdEndepointsAndLockFilters = endpoints.MapGroup("/dishes/{dishId:guid}")
                .RequireAuthorization("RequireAdminFromCroatia")
                .AddEndpointFilter(new DishIsLockedFilter(new Guid("fd630a57-2352-4731-b25c-db9cc7601b16")))
                .AddEndpointFilter(new DishIsLockedFilter(new Guid("98929bd4-f099-41eb-a994-f1918b724b5a")));

            dishesEndPoints.MapGet("", DishesHandlers.GetDishesAsync);
            dishesWithGuidIdEndPoints.MapGet("", DishesHandlers.GetDishesByIdAsync)
                .WithName("GetDish")
                .WithOpenApi(operation =>
                {
                    operation.Deprecated = true;
                    return operation;
                })
                .WithSummary("Get Dish by providing an id.")
.WithDescription("This endpoint retrieves a specific dish by its unique identifier (GUID). " +
"It returns detailed information about the dish, including its name, ingredients, and any associated metadata. " +
"Ensure that the provided ID corresponds to an existing dish in the database.");

            dishesEndPoints.MapGet("/{dishName}", DishesHandlers.GetDishesByNameAsync).AllowAnonymous();

            dishesEndPoints.MapPost("", DishesHandlers.CreateDishAsync)
                .RequireAuthorization("RequireAdminFromCroatia")
                .AddEndpointFilter<ValidateAnnotationsFilter>()
                .ProducesValidationProblem((int)HttpStatusCode.BadRequest)
                .Accepts<DishPostDto>(
                "application/json",
                "application/vnd.marvin.dishpostdto+json");

            dishWithGuidIdEndepointsAndLockFilters.MapPut("", DishesHandlers.UpdateDishAsync)
                .AddEndpointFilter<ValidateAnnotationsFilter>();

            dishWithGuidIdEndepointsAndLockFilters.MapDelete("", DishesHandlers.DeleteDishAsync)
                .AddEndpointFilter<LogNotFoundResponseFilter>();

        }

        public static void RegisterIngredientsEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var ingredientsEndPoints = endpoints.MapGroup("/dishes/{dishId:guid}/ingredients").RequireAuthorization();

            ingredientsEndPoints.MapGet("", IngredientsHandlers.GetIngredientsAsync);

            ingredientsEndPoints.MapPost("", () =>
            {
                throw new NotImplementedException();
            });
        }
    }
}
