using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace RaceDay.Api.Tests
{
    // start of class
    public class EnrolmentAndResultsTests
    {
        // Register and log in a test account.
        private static async Task<HttpClient> CreateLoggedInClient(
            CustomWebApplicationFactory factory,
            string role,
            string email,
            string idNumber)
        {
            var client = factory.CreateClient();

            var registration = new
            {
                FullName = "RaceDay Test User",
                Email = email,
                Password = "RaceDayTest123!",
                Role = role,
                IdNum = role == "Participant" ? idNumber : null,
                Address = role == "Participant" ? "Johannesburg" : null,
                PhoneNum = role == "Organiser" ? "0712345678" : null
            };

            var registerResponse = await client.PostAsJsonAsync(
                "/api/auth/register", registration);

            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await client.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    Email = email,
                    Password = "RaceDayTest123!"
                });

            loginResponse.EnsureSuccessStatusCode();

            return client;
        }

        // Create an event using the logged-in organiser.
        private static async Task<int> CreateEvent(
            HttpClient organiser)
        {
            var response = await organiser.PostAsJsonAsync(
                "/api/events",
                new
                {
                    EventName = "RaceDay Test Event",
                    EventDescription = "Event created for API tests",
                    Date = DateTime.Today.AddDays(30),
                    Location = "Johannesburg",
                    DistanceKm = 5.00m,
                    EventType = "Run"
                });

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            return json.GetProperty("eventId").GetInt32();
        }

        // Create a distance category for an event.
        private static async Task<int> CreateCategory(
            HttpClient organiser,
            int eventId)
        {
            var response = await organiser.PostAsJsonAsync(
                $"/api/events/{eventId}/categories",
                new
                {
                    CategoryName = "5 km Open",
                    Description = "Test distance category",
                    CategoryType = "Distance",
                    AgeGroup = (string?)null,
                    DistanceKm = 5.00m
                });

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            return json.GetProperty("categoryId").GetInt32();
        }

        // Start of method
        [Fact]
        public async Task Participant_CanEnrolInCategory()
        {
            using var factory = new CustomWebApplicationFactory();

            using var organiser = await CreateLoggedInClient(
                factory,
                "Organiser",
                "enrol.organiser@test.com",
                "");

            int eventId = await CreateEvent(organiser);
            int categoryId = await CreateCategory(organiser, eventId);

            using var participant = await CreateLoggedInClient(
                factory,
                "Participant",
                "enrol.participant@test.com",
                "9001015009087");

            var response = await participant.PostAsJsonAsync(
                "/api/enrolments",
                new { CategoryId = categoryId });

            Assert.Equal(
                HttpStatusCode.Created, response.StatusCode);
        }
        // End of method

        // Start of method
        [Fact]
        public async Task Participant_CannotEnrolTwiceInSameCategory()
        {
            using var factory = new CustomWebApplicationFactory();

            using var organiser = await CreateLoggedInClient(
                factory,
                "Organiser",
                "duplicate.organiser@test.com",
                "");

            int eventId = await CreateEvent(organiser);
            int categoryId = await CreateCategory(organiser, eventId);

            using var participant = await CreateLoggedInClient(
                factory,
                "Participant",
                "duplicate.participant@test.com",
                "9001015009087");

            var request = new { CategoryId = categoryId };

            var firstResponse = await participant.PostAsJsonAsync(
                "/api/enrolments", request);

            Assert.Equal(
                HttpStatusCode.Created, firstResponse.StatusCode);

            var secondResponse = await participant.PostAsJsonAsync(
                "/api/enrolments", request);

            // The API should reject a duplicate enrolment.
            Assert.True(
                secondResponse.StatusCode == HttpStatusCode.Conflict ||
                secondResponse.StatusCode == HttpStatusCode.BadRequest,
                $"Expected Conflict or BadRequest, but received {(int)secondResponse.StatusCode}.");
        }
        // End of method

        // Start of method
        [Fact]
        public async Task Participant_CannotViewAnotherParticipantsResults()
        {
            using var factory = new CustomWebApplicationFactory();

            using var organiser = await CreateLoggedInClient(
                factory,
                "Organiser",
                "results.organiser@test.com",
                "");

            int eventId = await CreateEvent(organiser);
            int categoryId = await CreateCategory(organiser, eventId);

            // First participant enrols in the category.
            using var participantOne = await CreateLoggedInClient(
                factory,
                "Participant",
                "results.first@test.com",
                "9001015009087");

            var enrolmentResponse = await participantOne.PostAsJsonAsync(
                "/api/enrolments",
                new { CategoryId = categoryId });

            enrolmentResponse.EnsureSuccessStatusCode();

            var enrolmentJson = await enrolmentResponse.Content
                .ReadFromJsonAsync<JsonElement>();

            int enrolmentId = enrolmentJson
                .GetProperty("enrolId").GetInt32();

            // The organiser records a result for that enrolment.
            var resultResponse = await organiser.PostAsJsonAsync(
                "/api/results",
                new
                {
                    EnrolId = enrolmentId,
                    Duration = "00:30:00",
                    AverageSpeed = 10.00m,
                    Position = 1
                });

            resultResponse.EnsureSuccessStatusCode();

            // A different participant signs in and checks their results.
            using var participantTwo = await CreateLoggedInClient(
                factory,
                "Participant",
                "results.second@test.com",
                "9001015009088");

            var response = await participantTwo.GetAsync(
                "/api/results/mine");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var results = await response.Content
                .ReadFromJsonAsync<JsonElement>();

            // The second participant must not see the first participant's result.
            Assert.Equal(JsonValueKind.Array, results.ValueKind);
            Assert.Empty(results.EnumerateArray());
        }
        // End of method
    }
    // End of class
}
// End of namespace
// end of code
