using Microsoft.AspNetCore.Identity;
using RaceDay.Api.Models;

namespace RaceDay.Api.Tests
{
    // start of class
    public class UnitTest1
    {
        // start of method
        [Fact]
        public void PasswordHasher_ShouldVerifyCorrectPassword()
        {
            // Arrange
            var user = new User();
            var hasher = new PasswordHasher<User>();
            string password = "RaceDayTest123!";

            // Act
            string passwordHash = hasher.HashPassword(user, password);
            var result = hasher.VerifyHashedPassword(
                user, passwordHash, password);

            // Assert
            Assert.NotNull(passwordHash);
            Assert.Equal(
                PasswordVerificationResult.Success, result);
        }// end of method


        // start of method
        [Fact]
        public void PasswordHasher_ShouldRejectIncorrectPassword()
        {
            // Arrange
            var user = new User();
            var hasher = new PasswordHasher<User>();
            string password = "RaceDayTest123!";
            string incorrectPassword = "WrongPassword123!";

            // Act
            string passwordHash = hasher.HashPassword(user, password);
            var result = hasher.VerifyHashedPassword(
                user, passwordHash, incorrectPassword);

            // Assert
            Assert.Equal(
                PasswordVerificationResult.Failed, result);
        }// end of method

    }// end of class

}// end of namespace