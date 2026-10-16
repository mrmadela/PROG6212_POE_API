using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{
    
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {// start of class UsersController
        private readonly RaceDayDbContext _db;

        // start of method: UsersController
        public UsersController(RaceDayDbContext db)
        {
            _db = db;
        } // end of method


        // GetProfile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {// start of method
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized(new
                {
                    message = "Please log in first."
                });
            }

            var user = await _db.Users
                .Include(u => u.Organiser)
                .Include(u => u.Participant)
                .FirstOrDefaultAsync(u => u.UserId == userId.Value);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var profile = new ProfileDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role,
                FullName = user.Organiser?.FullName
                    ?? user.Participant?.FullName
                    ?? string.Empty,
                PhoneNum = user.Organiser?.PhoneNum,
                IdNum = user.Participant?.IdNum,
                Address = user.Participant?.Address
            };

            return Ok(profile);
        }// end of method


        // Update Profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(
            UpdateProfileRequest request)
        {// start of UpdateProfile method
            // Get the ID of the currently logged-in user from the session.
            int? userId = HttpContext.Session.GetInt32("UserId");

            // Stop the request if the user is not logged in.
            if (userId == null)
            {
                return Unauthorized(new
                {
                    message = "Please log in first."
                });
            }

            // Validate the full name before calling Trim() or updating the database.
            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                return BadRequest(new
                {
                    message = "Full name is required."
                });
            }

            // Retrieve the user and their related organiser or participant profile.
            var user = await _db.Users
                .Include(u => u.Organiser)
                .Include(u => u.Participant)
                .FirstOrDefaultAsync(u => u.UserId == userId.Value);

            // Return an error if the logged-in user's account cannot be found.
            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            // Update only the profile associated with the user's assigned role.
            if (user.Role == "Organiser" && user.Organiser != null)
            {
                // Organisers must provide a phone number.
                if (string.IsNullOrWhiteSpace(request.PhoneNum))
                {
                    return BadRequest(new
                    {
                        message = "Phone number is required."
                    });
                }

                // Remove unnecessary spaces and update the organiser's details.
                user.Organiser.FullName = request.FullName.Trim();
                user.Organiser.PhoneNum = request.PhoneNum.Trim();
            }
            else if (user.Role == "Participant"
                     && user.Participant != null)
            {
                // Participants must provide an address.
                if (string.IsNullOrWhiteSpace(request.Address))
                {
                    return BadRequest(new
                    {
                        message = "Address is required."
                    });
                }

                // Update the participant's name and address.
                user.Participant.FullName = request.FullName.Trim();
                user.Participant.Address = request.Address.Trim();
            }
            else
            {
                // Report a configuration problem if the related profile is missing.
                return Conflict(new
                {
                    message = "The user profile is not configured correctly."
                });
            }

            // Save the validated profile changes to the database.
            await _db.SaveChangesAsync();

            // Confirm that the profile was updated successfully.
            return Ok(new
            {
                message = "Profile updated successfully."
            });
        }// end of UpdateProfile() method


    }// end of class
}// end of namespace


