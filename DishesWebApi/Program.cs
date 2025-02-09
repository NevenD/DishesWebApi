using DishesAPI.DbContexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddDbContext<DishesDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));



var app = builder.Build();

app.UseHttpsRedirection();



app.MapGet("/dishes", async (DishesDbContext context) =>
{
    return await context.Dishes.ToListAsync();
});

app.MapGet("/dishes/{dishId:guid}", async (DishesDbContext context, Guid dishId) =>
{
    return await context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
});

app.MapGet("/dishes/{dishName}", async (DishesDbContext context, string dishName) =>
{
    return await context.Dishes.FirstOrDefaultAsync(d => d.Name == dishName);
});

app.MapGet("/dishes/{dishId}/ingredients", async (DishesDbContext context, Guid dishId) =>
{
    return (await context.Dishes
    .Include(d => d.Ingredients)
    .FirstOrDefaultAsync(d => d.Id == dishId))?.Ingredients;
});

// recreate & migrate the database on each run
using (var serviceScope = app.Services?.GetService<IServiceScopeFactory>()?.CreateScope())
{
    var context = serviceScope?.ServiceProvider.GetRequiredService<DishesDbContext>();
    context?.Database.EnsureDeleted();
    context?.Database.Migrate();
}
app.Run();


