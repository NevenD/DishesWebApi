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

            dishesEndPoints.MapGet("", DishesHandlers.GetDishesAsync);
            dishesWithGuidIdEndPoints.MapGet("", DishesHandlers.GetDishesByIdAsync).WithName("GetDish");
            dishesEndPoints.MapGet("/{dishName}", DishesHandlers.GetDishesByNameAsync);
            dishesEndPoints.MapPost("", DishesHandlers.CreateDishAsync);
            dishesWithGuidIdEndPoints.MapPut("", DishesHandlers.UpdateDishAsync)
                .AddEndpointFilter<RendangDishIsLockedFilter>();
            dishesWithGuidIdEndPoints.MapDelete("", DishesHandlers.DeleteDishAsync)
                .AddEndpointFilter<RendangDishIsLockedFilter>(); ;

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
