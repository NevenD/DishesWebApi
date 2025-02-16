using DishesWebApi.EndPointFilters;
using DishesWebApi.EndpointHandlers;

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
                .RequireAuthorization()
                .AddEndpointFilter(new DishIsLockedFilter(new Guid("fd630a57-2352-4731-b25c-db9cc7601b16")))
                .AddEndpointFilter(new DishIsLockedFilter(new Guid("98929bd4-f099-41eb-a994-f1918b724b5a")));

            dishesEndPoints.MapGet("", DishesHandlers.GetDishesAsync);
            dishesWithGuidIdEndPoints.MapGet("", DishesHandlers.GetDishesByIdAsync).WithName("GetDish");
            dishesEndPoints.MapGet("/{dishName}", DishesHandlers.GetDishesByNameAsync).AllowAnonymous();

            dishesEndPoints.MapPost("", DishesHandlers.CreateDishAsync)
                .AddEndpointFilter<ValidateAnnotationsFilter>();
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
