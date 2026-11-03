using RaceDay.Mvc.Services;

var builder = WebApplication.CreateBuilder(args);

// start of service configuration
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".RaceDay.Mvc.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    string? apiBaseUrl =
        builder.Configuration["ApiSettings:BaseUrl"];

    if (string.IsNullOrWhiteSpace(apiBaseUrl))
    {
        throw new InvalidOperationException(
            "RaceDay API BaseUrl is not configured.");
    }

    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});
// end of service configuration

var app = builder.Build();

// start of middleware configuration
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// end of middleware configuration

app.Run();
