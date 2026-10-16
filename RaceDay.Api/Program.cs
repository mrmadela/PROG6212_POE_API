
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Connect to the RaceDayDB SQL Server database.
builder.Services.AddDbContext<RaceDayDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register controllers and Swagger.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register password hashing for User accounts.
builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>>();

// Register session services BEFORE builder.Build().
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".RaceDay.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

    // Use HTTPS for secure session cookies.
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

var app = builder.Build();

// Configure Swagger in development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable session before mapping controllers.
app.UseSession();

app.UseAuthorization();

app.MapControllers();

// Check whether the database is accessible.
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

    return Results.Problem(
        "Could not connect to RaceDayDB.");
});

app.Run();

// Make the API available to the test project
public partial class Program
{
} 

