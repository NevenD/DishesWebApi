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
                .AddEndpointFilter(async (context, next) =>
                {
                    var dishId = context.GetArgument<Guid>(2);
                    var rendangId = new Guid("fd630a57-2352-4731-b25c-db9cc7601b16");

                    if (dishId == rendangId)
                    {
                        return TypedResults.Problem(new()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Rendang is not allowed to be updated",
                            Detail = "Rendang is a national dish of Indonesia and should not be updated"
                        });
                    }
                    // invoke the next filter in the pipeline
                    var result = await next.Invoke(context);

                    return result;
                });
            dishesWithGuidIdEndPoints.MapDelete("", DishesHandlers.DeleteDishAsync);

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
