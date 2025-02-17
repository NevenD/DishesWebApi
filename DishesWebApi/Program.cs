using DishesAPI.DbContexts;
using DishesWebApi.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<DishesDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddProblemDetails();
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminFromCroatia", policy =>
    {
        policy
        .RequireRole("admin")
        .RequireClaim("country", "Croatia");
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// token with claims
//eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6Ik5ldmVuIiwic3ViIjoiTmV2ZW4iLCJqdGkiOiI2OTM5NTVjNCIsInJvbGUiOiJhZG1pbiIsImNvdW50cnkiOiJDcm9hdGlhIiwiYXVkIjoibWVudS1hcGkiLCJuYmYiOjE3Mzk3MzM0MzYsImV4cCI6MTc0NzQyMzAzNiwiaWF0IjoxNzM5NzMzNDM2LCJpc3MiOiJkb3RuZXQtdXNlci1qd3RzIn0.EOdUrHO9fgv7sbpDawv6h_iQt9LA3iZEnoUPzu_3Zac

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

app.UseSwagger();
app.UseSwaggerUI();

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


