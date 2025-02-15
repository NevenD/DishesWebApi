
namespace DishesWebApi.EndPointFilters
{
    public class DishIsLockedFilter : IEndpointFilter
    {
        private readonly Guid _lockedDishId;

        public DishIsLockedFilter(Guid lockedDishId)
        {
            _lockedDishId = lockedDishId;
        }


        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var dishId = context.GetArgument<Guid>(1);

            if (dishId == _lockedDishId)
            {
                return TypedResults.Problem(new()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Dish cannot be changed",
                    Detail = "You cannot update or delete perfection"
                });
            }
            // invoke the next filter in the pipeline
            var result = await next.Invoke(context);

            return result;
        }
    }
}
