
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Controllers;
using RaceDay.Api.Data;
using Xunit;

namespace RaceDay.Api.Tests
{
    // Contains unit tests for the user profile endpoints.
    public class ProfileControllerTests
    {
        [Fact]
        public async Task GetProfile_WithoutSession_ReturnsUnauthorized()
        {
            // Create a separate in-memory database for this test.
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            // Create the database context using the test database.
            using var db = new RaceDayDbContext(options);

            // Create the UsersController with the database context.
            var controller = new UsersController(db);

            // Create an HTTP context to simulate an incoming request.
            var httpContext = new DefaultHttpContext();

            // Attach an empty session to simulate a user who is not logged in.
            httpContext.Session = new TestSession();

            // Provide the HTTP context to the controller.
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Call GetProfile without a UserId stored in the session.
            var result = await controller.GetProfile();

            // Confirm that the endpoint returns HTTP 401 Unauthorized.
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // Provides a simple empty session for unit testing.
        private sealed class TestSession : ISession
        {
            // Indicates that the test session is available.
            public bool IsAvailable => true;

            // Provides an identifier for the test session.
            public string Id => Guid.NewGuid().ToString();

            // Returns the keys stored in the session.
            // This test session contains no stored values.
            public System.Collections.Generic.IEnumerable<string> Keys
                => Array.Empty<string>();

            // Removes all values from the session.
            public void Clear()
            {
            }

            // Simulates successfully saving the session.
            public Task CommitAsync(
                System.Threading.CancellationToken cancellationToken = default)
                => Task.CompletedTask;

            // Simulates successfully loading the session.
            public Task LoadAsync(
                System.Threading.CancellationToken cancellationToken = default)
                => Task.CompletedTask;

            // Attempts to retrieve a value from the session.
            // Returns false because the test session is empty.
            public bool TryGetValue(string key, out byte[] value)
            {
                value = null!;
                return false;
            }

            // Adds a value to the session.
            // No storage is needed for this test.
            public void Set(string key, byte[] value)
            {
            }

            // Removes a value from the session.
            // No storage is needed for this test.
            public void Remove(string key)
            {
            }
        }
    }
}
