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

            // Update only the profile associated with this account.
            if (user.Role == "Organiser" && user.Organiser != null)
            {
                user.Organiser.FullName = request.FullName.Trim();
                user.Organiser.PhoneNum = request.PhoneNum;
            }
            else if (user.Role == "Participant" && user.Participant != null)
            {
                user.Participant.FullName = request.FullName.Trim();
                user.Participant.Address = request.Address;
            }
            else
            {
                return Conflict(new
                {
                    message = "The user profile is not configured correctly."
                });
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile updated successfully."
            });
        }// end of UpdateProfile method

    }// end of class
}// end of namespace


