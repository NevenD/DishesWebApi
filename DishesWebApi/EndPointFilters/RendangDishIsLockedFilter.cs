
namespace DishesWebApi.EndPointFilters
{
    public class RendangDishIsLockedFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var dishId = context.GetArgument<Guid>(1);
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
        }
    }
}
