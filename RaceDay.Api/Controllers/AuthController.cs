

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{// start of namespace 
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {// start of class 
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
        public async Task<IActionResult> Register(
           [FromQuery] string email,
           [FromQuery] string password,
           [FromQuery] string role,
           [FromQuery] string? fullName,
           [FromQuery] string? phoneNum,
           [FromQuery] string? idNum,
           [FromQuery] string? address)
        {
            // Store the registration parameters in the DTO.
            var request = new RegisterRequest
            {
                Email = email,
                Password = password,
                Role = role,
                FullName = fullName,
                PhoneNum = phoneNum,
                IdNum = idNum,
                Address = address
            };

            // Validate required information.
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.Role))
            {
                return BadRequest(new
                {
                    message = "Email, password and role are required."
                });
            }

            string normalizedEmail =
                request.Email.Trim().ToLowerInvariant();

            string normalizedRole = request.Role.Trim();

            if (normalizedRole != "Organiser" &&
                normalizedRole != "Participant")
            {
                return BadRequest(new
                {
                    message = "Role must be Organiser or Participant."
                });
            }

            if (request.Password.Length < 8)
            {
                return BadRequest(new
                {
                    message = "Password must contain at least 8 characters."
                });
            }

            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                return BadRequest(new
                {
                    message = "Full name is required."
                });
            }

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



        // POST: /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromQuery] string email,
            [FromQuery] string password)
        {
            // Store the login parameters in the DTO.
            var request = new LoginRequest
            {
                Email = email,
                Password = password
            };

            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    message = "Email and password are required."
                });
            }

            string normalizedEmail =
                request.Email.Trim().ToLowerInvariant();

            var user = await _db.Users
                .FirstOrDefaultAsync(
                    u => u.Email == normalizedEmail);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            // Verify the submitted password against its stored hash.
            var verification = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

            if (verification == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            // Store the authenticated user's details in the session.
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", user.Role);

            return Ok(new AuthResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role,
                Message = "Login successful."
            });
        }


        // POST: /api/auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return Ok(new
            {
                message = "Logout successful."
            });
        }


    }// end of class
}// end of namespace 

