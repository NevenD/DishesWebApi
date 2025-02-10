using DishesAPI.DbContexts;
using DishesWebApi.DTOs;
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
});

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

// recreate & migrate the database on each run
using (var serviceScope = app.Services?.GetService<IServiceScopeFactory>()?.CreateScope())
{
    var context = serviceScope?.ServiceProvider.GetRequiredService<DishesDbContext>();
    context?.Database.EnsureDeleted();
    context?.Database.Migrate();
}
app.Run();


