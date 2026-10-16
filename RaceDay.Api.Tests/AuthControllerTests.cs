
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace RaceDay.Api.Tests
{
    public class AuthControllerTests
    {
        // Builds a URL containing the supplied query parameters.
        // Values are URL-encoded to handle special characters safely.
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

        // Registers a participant using URL query parameters.
        private static async System.Threading.Tasks.Task<
            System.Net.Http.HttpResponseMessage> RegisterParticipantAsync(
                System.Net.Http.HttpClient client,
                string fullName,
                string email,
                string password,
                string idNum)
        {
            var url = BuildQueryUrl(
                "/api/auth/register",
                new Dictionary<string, string?>
                {
                    ["fullName"] = fullName,
                    ["email"] = email,
                    ["password"] = password,
                    ["role"] = "Participant",
                    ["idNum"] = idNum,
                    ["address"] = "Johannesburg",
                    ["phoneNum"] = null
                });

            return await client.PostAsync(url, null);
        }

        [Fact]
        public async System.Threading.Tasks.Task
            Register_ValidParticipant_ReturnsCreated()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            using var client = factory.CreateClient();

            // Act
            var response = await RegisterParticipantAsync(
                client,
                "Test Participant",
                "participant@test.com",
                "RaceDayTest123!",
                "9001015009087");

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async System.Threading.Tasks.Task
            Register_DuplicateEmail_ReturnsConflict()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            using var client = factory.CreateClient();

            // Use the same email twice, but a different ID for the
            // second attempt so the duplicate email is the key conflict.
            var firstResponse = await RegisterParticipantAsync(
                client,
                "Test Participant",
                "duplicate@test.com",
                "RaceDayTest123!",
                "9001015009087");

            // Act
            var secondResponse = await RegisterParticipantAsync(
                client,
                "Test Participant",
                "duplicate@test.com",
                "RaceDayTest123!",
                "9102025009088");

            // Assert
            Assert.Equal(
                HttpStatusCode.Created,
                firstResponse.StatusCode);

            Assert.Equal(
                HttpStatusCode.Conflict,
                secondResponse.StatusCode);
        }

        [Fact]
        public async System.Threading.Tasks.Task
            Login_ValidCredentials_ReturnsOk()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            using var client = factory.CreateClient();

            await RegisterParticipantAsync(
                client,
                "Login Participant",
                "login@test.com",
                "RaceDayTest123!",
                "9203035009089");

            var loginUrl = BuildQueryUrl(
                "/api/auth/login",
                new Dictionary<string, string?>
                {
                    ["email"] = "login@test.com",
                    ["password"] = "RaceDayTest123!"
                });

            // Act
            var response = await client.PostAsync(loginUrl, null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async System.Threading.Tasks.Task
            Login_IncorrectPassword_ReturnsUnauthorized()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            using var client = factory.CreateClient();

            await RegisterParticipantAsync(
                client,
                "Login Participant",
                "wrongpassword@test.com",
                "RaceDayTest123!",
                "9304045009080");

            var loginUrl = BuildQueryUrl(
                "/api/auth/login",
                new Dictionary<string, string?>
                {
                    ["email"] = "wrongpassword@test.com",
                    ["password"] = "IncorrectPassword123!"
                });

            // Act
            var response = await client.PostAsync(loginUrl, null);

            // Assert
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async System.Threading.Tasks.Task
            GetProfile_WithoutLogin_ReturnsUnauthorized()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            using var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync(
                "/api/users/profile");

            // Assert
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }
    }
}
