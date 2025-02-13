using DishesAPI.DbContexts;
using DishesAPI.Entities;
using DishesWebApi.DTOs;
using DishesWebApi.Entities;
using DishesWebApi.Mappings;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DishesWebApi.EndpointHandlers
{
    public static class DishesHandlers
    {
        public static async Task<Ok<IEnumerable<DishDto>>> GetDishesAsync(DishesDbContext context, ILogger<DishDto> logger, string? name)
        {
            var dishes = await context.Dishes.ToListAsync();
            var filteredDishes = dishes.Where(x => name == null || x.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).Select(x => x.ToDishDto());

            logger.LogInformation("Getting all dishes");


            return TypedResults.Ok(filteredDishes);
        }

        public static async Task<Results<NotFound, Ok<DishDto>>> GetDishesByIdAsync(DishesDbContext context, Guid dishId)
        {
            var dish = await context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
            if (dish is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(dish.ToDishDto());
        }

        public static async Task<Results<NotFound, Ok<DishDto>>> GetDishesByNameAsync(DishesDbContext context, string dishName)
        {
            var dish = await context.Dishes.FirstOrDefaultAsync(d => d.Name == dishName);
            if (dish is null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(dish.ToDishDto());
        }

        public static async Task<CreatedAtRoute<DishDto>> CreateDishAsync(DishesDbContext context, [FromBody] DishPostDto dishPost /*LinkGenerator linkGenerator, HttpContext httpContext*/)
        {
            var dish = new Dish
            {
                Name = dishPost.Name,
            };

            context.Add(dish);
            await context.SaveChangesAsync();

            var dishToReturn = dish.ToDishDto();

            return TypedResults.CreatedAtRoute(dishToReturn, "GetDish", new { dishId = dishToReturn.Id });

            //var linkToDish = linkGenerator.GetUriByName(httpContext, "GetDish", new {dishId = dishToReturn.Id });
            // return TypedResults.Created(linkToDish, dishToReturn);
        }

        public static async Task<Results<NotFound, NoContent>> UpdateDishAsync(DishesDbContext context, Guid dishId, DishPutDto dishPut)
        {
            var dish = await context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
            if (dish is null)
            {
                return TypedResults.NotFound();
            }

            dish.Name = dishPut.Name;
            await context.SaveChangesAsync();

            return TypedResults.NoContent();
        }

        public static async Task<Results<NotFound, NoContent>> DeleteDishAsync(DishesDbContext context, Guid dishId)
        {
            var dish = await context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
            if (dish is null)
            {
                return TypedResults.NotFound();
            }

            context.Dishes.Remove(dish);
            await context.SaveChangesAsync();

            return TypedResults.NoContent();
        }
    }
}
