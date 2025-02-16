using DishesAPI.DbContexts;
using DishesWebApi.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<DishesDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddProblemDetails();
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{

    app.UseExceptionHandler();
    //// if we add builder.Services.AddProblemDetails(); then this will not be needed
    //// order is important, this middleware is meant to catch all exceptions in other pieces of middler ware
    //// and bubble back up to the request pipeline
    //// should be at begginning
    //app.UseExceptionHandler(cfb =>
    //{
    //    cfb.Run(
    //        async context =>
    //        {
    //            // The line sets the HTTP response status code to 500 (Internal Server Error) 
    //            // whenever an exception occurs in the application.
    //            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    //            context.Response.ContentType = "text/html";
    //            await context.Response.WriteAsync("An unexpected issue happened. Try again later.");
    //        });

    //});
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.RegisterDishesEndpoints();
app.RegisterIngredientsEndpoints();

// recreate & migrate the database on each run
using (var serviceScope = app.Services?.GetService<IServiceScopeFactory>()?.CreateScope())
{
    var context = serviceScope?.ServiceProvider.GetRequiredService<DishesDbContext>();
    context?.Database.EnsureDeleted();
    context?.Database.Migrate();
}
app.Run();


