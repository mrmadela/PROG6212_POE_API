using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Controllers;
using RaceDay.Api.Data;
using RaceDay.Contracts;
using Xunit;

namespace RaceDay.Api.Tests
{
    
    public class EventRoleTests
    {// start of class

        [Fact]
        public async Task Participant_CannotCreateEvent_ReturnsForbidden()
        { // start of method
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var db = new RaceDayDbContext(options);
            var controller = new EventsController(db);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession();
            httpContext.Session.SetInt32("UserId", 1);
            httpContext.Session.SetString("Role", "Participant");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var request = new CreateEventRequest
            {
                EventName = "Test Race",
                EventDescription = "Test event",
                Date = DateTime.Today.AddDays(10),
                Location = "Johannesburg",
                DistanceKm = 5,
                EventType = "Run"
            };

            var result = await controller.CreateEvent(
                request.EventName,
                request.EventDescription,
                request.Date,
                request.Location,
                request.DistanceKm,
                request.EventType
            );

            Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, ((ObjectResult)result).StatusCode);
        }// end of method

    }// end of class



    public class TestSession : ISession
    {// start of class
        private readonly Dictionary<string, byte[]> _values = new();

        public bool IsAvailable => true;
        public string Id => "test-session";
        public IEnumerable<string> Keys => _values.Keys;

        public void Clear() => _values.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;
        public void Remove(string key) => _values.Remove(key);
        public void Set(string key, byte[] value) => _values[key] = value;
        public bool TryGetValue(string key, out byte[] value)
            => _values.TryGetValue(key, out value!);
    }// end of class

}// end of namespace

