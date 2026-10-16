
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace RaceDay.Api.Tests
{
    public class EventPermissionTests
    {
        // Builds a URL containing encoded query parameters.
        private static string BuildQueryUrl(
            string endpoint,
            Dictionary<string, string?> parameters)
        {
            var query = string.Join("&",
                parameters
                    .Where(p => p.Value != null)
                    .Select(p =>
                        $"{Uri.EscapeDataString(p.Key)}=" +
                        $"{Uri.EscapeDataString(p.Value!)}"));

            return $"{endpoint}?{query}";
        }

        // Creates an account and logs in.
        // The client retains its session cookie between requests.
        private static async Task<HttpClient> CreateLoggedInClient(
            CustomWebApplicationFactory factory,
            string role,
            string email,
            string idNumber)
        {
            var client = factory.CreateClient();

            var registration = new Dictionary<string, string?>
            {
                ["fullName"] = "RaceDay Test User",
                ["email"] = email,
                ["password"] = "RaceDayTest123!",
                ["role"] = role,
                ["idNum"] = role == "Participant" ? idNumber : null,
                ["address"] = role == "Participant"
                    ? "Johannesburg"
                    : null,
                ["phoneNum"] = role == "Organiser"
                    ? "0712345678"
                    : null
            };

            var registerUrl = BuildQueryUrl(
                "/api/auth/register", registration);

            var registerResponse = await client.PostAsync(
                registerUrl, null);

            if (!registerResponse.IsSuccessStatusCode)
            {
                var error = await registerResponse.Content.ReadAsStringAsync();
                client.Dispose();

                throw new InvalidOperationException(
                    $"Registration failed with status " +
                    $"{(int)registerResponse.StatusCode} " +
                    $"{registerResponse.StatusCode}. Response: {error}");
            }

            var loginUrl = BuildQueryUrl(
                "/api/auth/login",
                new Dictionary<string, string?>
                {
                    ["email"] = email,
                    ["password"] = "RaceDayTest123!"
                });

            var loginResponse = await client.PostAsync(loginUrl, null);

            if (!loginResponse.IsSuccessStatusCode)
            {
                var error = await loginResponse.Content.ReadAsStringAsync();
                client.Dispose();

                throw new InvalidOperationException(
                    $"Login failed with status " +
                    $"{(int)loginResponse.StatusCode} " +
                    $"{loginResponse.StatusCode}. Response: {error}");
            }

            return client;
        }

        // Builds the event creation URL using query parameters.
        private static string BuildCreateEventUrl(
            string eventName,
            string? eventDescription,
            DateTime date,
            string location,
            decimal distanceKm,
            string eventType)
        {
            return BuildQueryUrl(
                "/api/events",
                new Dictionary<string, string?>
                {
                    ["eventName"] = eventName,
                    ["eventDescription"] = eventDescription,
                    ["date"] = date.ToString("O"),
                    ["location"] = location,
                    ["distanceKm"] = distanceKm.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    ["eventType"] = eventType
                });
        }

        [Fact]
        public async Task Participant_CannotCreateEvent()
        {
            using var factory = new CustomWebApplicationFactory();

            using var client = await CreateLoggedInClient(
                factory,
                "Participant",
                "participant.event@test.com",
                "9001015009087");

            var eventUrl = BuildCreateEventUrl(
                "Test Fun Run",
                "Automated test event",
                DateTime.Today.AddDays(30),
                "Johannesburg",
                5.00m,
                "Run");

            var response = await client.PostAsync(eventUrl, null);

            Assert.Equal(
                HttpStatusCode.Forbidden,
                response.StatusCode);
        }

        [Fact]
        public async Task Organiser_CanCreateOwnEvent()
        {
            using var factory = new CustomWebApplicationFactory();

            using var client = await CreateLoggedInClient(
                factory,
                "Organiser",
                "organiser.create@test.com",
                "");

            var eventUrl = BuildCreateEventUrl(
                "Test Organiser Run",
                "Created during automated testing",
                DateTime.Today.AddDays(30),
                "Johannesburg",
                5.00m,
                "Run");

            var response = await client.PostAsync(eventUrl, null);

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);
        }

        [Fact]
        public async Task Organiser_CannotUpdateAnotherOrganisersEvent()
        {
            using var factory = new CustomWebApplicationFactory();

            // First organiser creates an event.
            using var firstOrganiser = await CreateLoggedInClient(
                factory,
                "Organiser",
                "organiser.owner@test.com",
                "");

            var eventUrl = BuildCreateEventUrl(
                "Owner Event",
                "Belongs to the first organiser",
                DateTime.Today.AddDays(30),
                "Johannesburg",
                5.00m,
                "Run");

            var createResponse = await firstOrganiser.PostAsync(
                eventUrl, null);

            Assert.Equal(
                HttpStatusCode.Created,
                createResponse.StatusCode);

            // Read the ID returned by the API.
            var createdEvent = await createResponse.Content
                .ReadFromJsonAsync<EventResponse>();

            Assert.NotNull(createdEvent);

            // A second organiser tries to update the first organiser's event.
            using var secondOrganiser = await CreateLoggedInClient(
                factory,
                "Organiser",
                "organiser.other@test.com",
                "");

            var updateUrl = BuildQueryUrl(
                $"/api/events/{createdEvent.EventId}",
                new Dictionary<string, string?>
                {
                    ["eventName"] = "Unauthorised Change",
                    ["eventDescription"] =
                        "This update should be blocked",
                    ["date"] = DateTime.Today.AddDays(40).ToString("O"),
                    ["location"] = "Pretoria",
                    ["distanceKm"] = 10.00m.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
                    ["eventType"] = "Run"
                });

            var updateResponse = await secondOrganiser.PutAsync(
                updateUrl, null);

            Assert.Equal(
                HttpStatusCode.Forbidden,
                updateResponse.StatusCode);
        }
    }

    // Represents the event ID returned by the API.
    public class EventResponse
    {
        public int EventId { get; set; }
    }
}
