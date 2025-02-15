using DishesWebApi.EndPointFilters;
using DishesWebApi.EndpointHandlers;

namespace DishesWebApi.Extensions
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void RegisterDishesEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var dishesEndPoints = endpoints.MapGroup("/dishes");
            var dishesWithGuidIdEndPoints = dishesEndPoints.MapGroup("/{dishId:guid}");
            var ingridentsEndPoints = dishesEndPoints.MapGroup("/ingredients");

            var dishWithGuidIdEndepointsAndLockFilters = dishesWithGuidIdEndPoints.MapGroup("/{dishId:guid}")
                .AddEndpointFilter(new DishIsLockedFilter(new Guid("fd630a57-2352-4731-b25c-db9cc7601b16")))
                .AddEndpointFilter(new DishIsLockedFilter(new Guid("98929bd4-f099-41eb-a994-f1918b724b5a")));

            dishesEndPoints.MapGet("", DishesHandlers.GetDishesAsync);
            dishesWithGuidIdEndPoints.MapGet("", DishesHandlers.GetDishesByIdAsync).WithName("GetDish");
            dishesEndPoints.MapGet("/{dishName}", DishesHandlers.GetDishesByNameAsync);
            dishesEndPoints.MapPost("", DishesHandlers.CreateDishAsync);

            dishWithGuidIdEndepointsAndLockFilters.MapPut("", DishesHandlers.UpdateDishAsync);
            dishWithGuidIdEndepointsAndLockFilters.MapDelete("", DishesHandlers.DeleteDishAsync);

        }

        public static void RegisterIngredientsEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var ingredientsEndPoints = endpoints.MapGroup("/dishes/{dishId:guid}/ingredients");

            ingredientsEndPoints.MapGet("", IngredientsHandlers.GetIngredientsAsync);

            ingredientsEndPoints.MapPost("", () =>
            {
                throw new NotImplementedException();
            });
        }
    }
}
