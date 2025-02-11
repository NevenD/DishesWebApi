using DishesAPI.DbContexts;
using DishesWebApi.DTOs;
using DishesWebApi.Mappings;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace DishesWebApi.EndpointHandlers
{
    public static class IngredientsHandlers
    {
        public static async Task<Results<NotFound, Ok<IEnumerable<IngredientDto>>>> GetIngredientsAsync(DishesDbContext context, Guid dishId)
        {
            var ingredients = (await context.Dishes.Include(d => d.Ingredients).FirstOrDefaultAsync(d => d.Id == dishId))?.Ingredients;

            if (ingredients is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(ingredients.Select(i => i.ToIngredientDto()));
        }
    }
}
