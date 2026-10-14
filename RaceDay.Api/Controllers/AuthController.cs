

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _db;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthController(
            RaceDayDbContext db,
            IPasswordHasher<User> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        // POST: /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            string email = request.Email.Trim().ToLowerInvariant();
            string role = request.Role.Trim();

            // Check whether the email is already registered.
            if (await _db.Users.AnyAsync(u => u.Email == email))
            {
                return Conflict(new
                {
                    message = "This email address is already registered."
                });
            }

            // Participants must provide an ID number.
            if (role == "Participant" &&
                string.IsNullOrWhiteSpace(request.IdNum))
            {
                return BadRequest(new
                {
                    message = "An ID number is required for participants."
                });
            }

            // Check whether the participant ID is already in use.
            if (role == "Participant" &&
                await _db.Participants.AnyAsync(
                    p => p.IdNum == request.IdNum))
            {
                return Conflict(new
                {
                    message = "This ID number is already registered."
                });
            }

            var user = new User
            {
                Email = email,
                Role = role
            };

            // Store a password hash, never the original password.
            user.PasswordHash =
                _passwordHasher.HashPassword(user, request.Password);

            if (role == "Organiser")
            {
                user.Organiser = new Organiser
                {
                    FullName = request.FullName.Trim(),
                    PhoneNum = request.PhoneNum
                };
            }
            else if (role == "Participant")
            {
                user.Participant = new Participant
                {
                    FullName = request.FullName.Trim(),
                    IdNum = request.IdNum!,
                    Address = request.Address
                };
            }
            else
            {
                return BadRequest(new
                {
                    message = "Role must be Organiser or Participant."
                });
            }

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return StatusCode(201, new AuthResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role,
                Message = "Registration successful."
            });
        }
    }
}

