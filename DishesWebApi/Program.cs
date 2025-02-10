using DishesAPI.DbContexts;
using DishesWebApi.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddDbContext<DishesDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));



var app = builder.Build();

app.UseHttpsRedirection();



app.MapGet("/dishes", async (DishesDbContext context, [FromQuery] string? name) =>
{
    var dishes = await context.Dishes.ToListAsync();
    return dishes.Where(x => name == null || x.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).Select(x => x.ToDishDto());
});

app.MapGet("/dishes/{dishId:guid}", async (DishesDbContext context, Guid dishId) =>
{
    var dish = await context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
    return dish?.ToDishDto();
});

app.MapGet("/dishes/{dishName}", async (DishesDbContext context, string dishName) =>
{
    var dish = await context.Dishes.FirstOrDefaultAsync(d => d.Name == dishName);
    return dish?.ToDishDto();
});

app.MapGet("/dishes/{dishId}/ingredients", async (DishesDbContext context, Guid dishId) =>
{
    var ingredients = (await context.Dishes.Include(d => d.Ingredients).FirstOrDefaultAsync(d => d.Id == dishId))?.Ingredients;
    return ingredients?.Select(i => i.ToIngredientDto());
});

// recreate & migrate the database on each run
using (var serviceScope = app.Services?.GetService<IServiceScopeFactory>()?.CreateScope())
{
    var context = serviceScope?.ServiceProvider.GetRequiredService<DishesDbContext>();
    context?.Database.EnsureDeleted();
    context?.Database.Migrate();
}
app.Run();


