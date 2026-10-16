using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RaceDay.Api.Data;

namespace RaceDay.Api.Tests
{
    
    public class CustomWebApplicationFactory
        : WebApplicationFactory<Program>
    {// start of class

        // Use a separate in-memory database for each factory.
        private readonly string _databaseName =
            Guid.NewGuid().ToString();

        // start of method
        protected override void ConfigureWebHost(
            IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Replace the real SQL Server database in tests.
                services.RemoveAll<RaceDayDbContext>();
                services.RemoveAll<
                    DbContextOptions<RaceDayDbContext>>();

                services.AddDbContext<RaceDayDbContext>(
                    options =>
                        options.UseInMemoryDatabase(_databaseName));
            });
        }// end of method

    }// end of class

}// end of namespace

