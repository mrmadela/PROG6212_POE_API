using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RaceDayDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<
    IPasswordHasher<RaceDay.Api.Models.User>,
    PasswordHasher<RaceDay.Api.Models.User>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/health/database", async (RaceDayDbContext db) =>
{
    bool connected = await db.Database.CanConnectAsync();

    if (connected)
    {
        return Results.Ok(new
        {
            message = "Database connection successful!",
            database = "RaceDayDB"
        });
    }

    return Results.Problem("Could not connect to RaceDayDB.");
});

app.Run();
