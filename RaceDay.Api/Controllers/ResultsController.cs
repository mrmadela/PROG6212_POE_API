using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{
    
    [ApiController]
    [Route("api/results")]
    public class ResultsController : ControllerBase
    {// start of class
        private readonly RaceDayDbContext _db;

        public ResultsController(RaceDayDbContext db)
        {
            _db = db;
        }


        // POST: /api/results
        [HttpPost]
        public async Task<IActionResult> CreateResult(
            [FromQuery] int enrolId,
            [FromQuery] TimeSpan? duration,
            [FromQuery] decimal? averageSpeed,
            [FromQuery] int? position)
        {// start of method
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized(new { message = "Please log in." });

            if (role != "Organiser")
                return StatusCode(403,
                    new { message = "Only Organisers can record results." });

            if (duration.HasValue && duration.Value < TimeSpan.Zero)
                return BadRequest(new { message = "Duration cannot be negative." });

            if (averageSpeed.HasValue && averageSpeed.Value < 0)
                return BadRequest(new { message = "Average speed cannot be negative." });

            if (position.HasValue && position.Value < 1)
                return BadRequest(new { message = "Position must be at least 1." });

            var organiser = await _db.Organisers
                .FirstOrDefaultAsync(o => o.UserId == userId.Value);

            if (organiser == null)
                return NotFound(new { message = "Organiser profile not found." });

            var enrolment = await _db.Enrolments
                .Include(e => e.Category)
                .ThenInclude(c => c.Event)
                .FirstOrDefaultAsync(e => e.EnrolId == enrolId);

            if (enrolment == null)
                return NotFound(new { message = "Enrolment not found." });

            if (enrolment.Category.Event.OrgId != organiser.OrgId)
                return StatusCode(403,
                    new { message = "You can only record results for your own events." });

            bool resultExists = await _db.Results
                .AnyAsync(r => r.EnrolId == enrolment.EnrolId);

            if (resultExists)
                return Conflict(new
                {
                    message = "A result already exists for this enrolment."
                });

            var result = new Result
            {
                EnrolId = enrolment.EnrolId,
                Duration = duration,
                AverageSpeed = averageSpeed,
                Position = position
            };

            _db.Results.Add(result);
            await _db.SaveChangesAsync();

            var response = new ResultDto
            {
                ResultId = result.ResultId,
                EnrolId = result.EnrolId,
                Duration = result.Duration,
                AverageSpeed = result.AverageSpeed,
                Position = result.Position
            };

            return StatusCode(201, response);
        }// end of method


        // GET: /api/results/mine
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyResults()
        {// start of method
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized(new { message = "Please log in." });

            if (role != "Participant")
                return StatusCode(403,
                    new { message = "Participant access required." });

            var participant = await _db.Participants
                .FirstOrDefaultAsync(p => p.UserId == userId.Value);

            if (participant == null)
                return NotFound(
                    new { message = "Participant profile not found." });

            var results = await _db.Results
                .AsNoTracking()
                .Where(r => r.Enrolment.ParticipantId == participant.ParticipantId)
                .Select(r => new ResultDto
                {
                    ResultId = r.ResultId,
                    EnrolId = r.EnrolId,
                    Duration = r.Duration,
                    AverageSpeed = r.AverageSpeed,
                    Position = r.Position
                })
                .ToListAsync();

            return Ok(results);
        }// end of method


        // GET: /api/results/event/5
        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult> GetEventResults(int eventId)
        {// start of method
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized(new { message = "Please log in." });

            if (role != "Organiser")
                return StatusCode(403,
                    new { message = "Organiser access required." });

            var organiser = await _db.Organisers
                .FirstOrDefaultAsync(o => o.UserId == userId.Value);

            if (organiser == null)
                return NotFound(
                    new { message = "Organiser profile not found." });

            var eventItem = await _db.Events
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            if (eventItem.OrgId != organiser.OrgId)
                return StatusCode(403,
                    new { message = "You can only view results for your own events." });

            var results = await _db.Results
                .AsNoTracking()
                .Where(r => r.Enrolment.Category.EventId == eventId)
                .Select(r => new ResultDto
                {
                    ResultId = r.ResultId,
                    EnrolId = r.EnrolId,
                    Duration = r.Duration,
                    AverageSpeed = r.AverageSpeed,
                    Position = r.Position
                })
                .ToListAsync();

            return Ok(results);
        }// end of method

    }// end of class

}// end of namespace


