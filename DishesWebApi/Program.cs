using DishesAPI.DbContexts;
using DishesAPI.Entities;
using DishesWebApi.DTOs;
using DishesWebApi.Entities;
using DishesWebApi.Mappings;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddDbContext<DishesDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));



var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/dishes", async Task<Ok<IEnumerable<DishDto>>> (DishesDbContext context, [FromQuery] string? name) =>
{
    var dishes = await context.Dishes.ToListAsync();
    var filteredDishes = dishes.Where(x => name == null || x.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).Select(x => x.ToDishDto());

    return TypedResults.Ok(filteredDishes);
});

app.MapGet("/dishes/{dishId:guid}", async Task<Results<NotFound, Ok<DishDto>>> (DishesDbContext context, Guid dishId) =>
{
    var dish = await context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);

    if (dish is null)
    {
        return TypedResults.NotFound();
    }

    return TypedResults.Ok(dish.ToDishDto());
}).WithName("GetDish");

app.MapGet("/dishes/{dishName}", async Task<Results<NotFound, Ok<DishDto>>> (DishesDbContext context, string dishName) =>
{
    var dish = await context.Dishes.FirstOrDefaultAsync(d => d.Name == dishName);
    if (dish is null)
    {
        return TypedResults.NotFound();
    }
    return TypedResults.Ok(dish.ToDishDto());
});

app.MapGet("/dishes/{dishId}/ingredients", async Task<Results<NotFound, Ok<IEnumerable<IngredientDto>>>> (DishesDbContext context, Guid dishId) =>
{
    var ingredients = (await context.Dishes.Include(d => d.Ingredients).FirstOrDefaultAsync(d => d.Id == dishId))?.Ingredients;

    if (ingredients is null)
    {
        return TypedResults.NotFound();
    }

    return TypedResults.Ok(ingredients.Select(i => i.ToIngredientDto()));
});

// [FromBody] is not needed, just as an example
app.MapPost("/dishes", async Task<CreatedAtRoute<DishDto>> (DishesDbContext context, [FromBody] DishPostDto dishPost /*LinkGenerator linkGenerator, HttpContext httpContext*/) =>
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
});


app.MapPut("/dishes/{dishId:Guid}", async Task<Results<NotFound, NoContent>> (DishesDbContext context, Guid dishId, DishPutDto dishPut) =>
{

    var dish = await context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
    if (dish is null)
    {
        return TypedResults.NotFound();
    }

    dish.Name = dishPut.Name;
    await context.SaveChangesAsync();

    return TypedResults.NoContent();
});


app.MapDelete("/dishes/{dishId:Guid}", async Task<Results<NotFound, NoContent>> (DishesDbContext context, Guid dishId) =>
{
    var dish = await context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
    if (dish is null)
    {
        return TypedResults.NotFound();
    }

    context.Dishes.Remove(dish);
    await context.SaveChangesAsync();

    return TypedResults.NoContent();
});

// recreate & migrate the database on each run
using (var serviceScope = app.Services?.GetService<IServiceScopeFactory>()?.CreateScope())
{
    var context = serviceScope?.ServiceProvider.GetRequiredService<DishesDbContext>();
    context?.Database.EnsureDeleted();
    context?.Database.Migrate();
}
app.Run();


