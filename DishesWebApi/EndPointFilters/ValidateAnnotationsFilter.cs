
using DishesWebApi.Entities;
using MiniValidation;

namespace DishesWebApi.EndPointFilters
{
    public class ValidateAnnotationsFilter : IEndpointFilter
    {

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var dishPost = context.GetArgument<DishPostDto>(1);

            if (!MiniValidator.TryValidate(dishPost, out var validationErrors))
            {
                return TypedResults.ValidationProblem(validationErrors);
            }

            return await next(context);
        }
    }
}
