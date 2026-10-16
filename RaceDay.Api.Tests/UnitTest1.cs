using Microsoft.AspNetCore.Identity;
using RaceDay.Api.Models;

namespace RaceDay.Api.Tests
{
    
    public class UnitTest1
    {// start of class

        [Fact]
        public void PasswordHasher_ShouldVerifyCorrectPassword()
        {// start of method
            // Arrange
            var user = new object();
            var hasher = new PasswordHasher<object> ();
            string password = "RaceDayTest123!";


        // Act
        string hash = hasher.HashPassword(user, password);
            var result = hasher.VerifyHashedPassword(
                user, hash, password);

            // Assert
            Assert.Equal(
                PasswordVerificationResult.Success, result);
        }// end of method


        
        [Fact]
        public void PasswordHasher_ShouldRejectIncorrectPassword()
        {// start of method
            // Arrange
            var user = new object();
            var hasher = new PasswordHasher<object>();
            string password = "RaceDayTest123!";
            string wrongPassword = "WrongPassword123!";

            // Act
            string hash = hasher.HashPassword(user, password);
            var result = hasher.VerifyHashedPassword(
                user, hash, wrongPassword);

            // Assert
            Assert.Equal(
                PasswordVerificationResult.Failed, result);
        }// end of method

    }// end of class



}// end of namespace

