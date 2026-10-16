using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{
    
    [ApiController]
    [Route("api/enrolments")]
    public class EnrolmentsController : ControllerBase
    {// start of class
        private readonly RaceDayDbContext _db;

        // start of method
        public EnrolmentsController(RaceDayDbContext db)
        {
            _db = db;
        }


        // POST: /api/enrolments?categoryId=1
        [HttpPost]
        public async Task<IActionResult> CreateEnrolment(
            [FromQuery] int categoryId)
        {// start of method
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized(new { message = "Please log in." });

            if (role != "Participant")
                return StatusCode(403,
                    new { message = "Only Participants can enrol." });

            var participant = await _db.Participants
                .FirstOrDefaultAsync(p => p.UserId == userId.Value);

            if (participant == null)
                return NotFound(new { message = "Participant profile not found." });

            var category = await _db.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category == null)
                return NotFound(new { message = "Category not found." });

            var eventItem = await _db.Events
                .FirstOrDefaultAsync(e => e.EventId == category.EventId);

            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            if (eventItem.Date.Date < DateTime.Today)
                return BadRequest(new { message = "You cannot enrol in a past event." });

            bool alreadyEnrolled = await _db.Enrolments.AnyAsync(e =>
                e.ParticipantId == participant.ParticipantId &&
                e.CategoryId == categoryId);

            if (alreadyEnrolled)
                return Conflict(new
                {
                    message = "You are already enrolled in this category."
                });

            var enrolment = new Enrolment
            {
                EnrolDate = DateTime.Today,
                CategoryId = category.CategoryId,
                ParticipantId = participant.ParticipantId
            };

            _db.Enrolments.Add(enrolment);
            await _db.SaveChangesAsync();

            var response = new EnrolmentDto
            {
                EnrolId = enrolment.EnrolId,
                EnrolDate = enrolment.EnrolDate,
                CategoryId = enrolment.CategoryId,
                ParticipantId = enrolment.ParticipantId
            };

            return StatusCode(201, response);
        }// end of method


        // GET: /api/enrolments/mine
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyEnrolments()
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

            var enrolments = await _db.Enrolments
                .AsNoTracking()
                .Where(e => e.ParticipantId == participant.ParticipantId)
                .Select(e => new EnrolmentDto
                {
                    EnrolId = e.EnrolId,
                    EnrolDate = e.EnrolDate,
                    CategoryId = e.CategoryId,
                    ParticipantId = e.ParticipantId
                })
                .ToListAsync();

            return Ok(enrolments);
        }// end of method


        // GET: /api/enrolments/event/5
        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult> GetEventEnrolments(int eventId)
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
                    new { message = "You can only view enrolments for your own events." });

            var enrolments = await _db.Enrolments
                .AsNoTracking()
                .Where(e => e.Category.EventId == eventId)
                .Select(e => new EnrolmentDto
                {
                    EnrolId = e.EnrolId,
                    EnrolDate = e.EnrolDate,
                    CategoryId = e.CategoryId,
                    ParticipantId = e.ParticipantId
                })
                .ToListAsync();

            return Ok(enrolments);
        }// end of method


        // DELETE: /api/enrolments/5
        // A participant may cancel their own enrolment before a result exists.
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> CancelEnrolment(int id)
        {// start of method
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized(new { message = "Please log in." });

            if (role != "Participant")
                return StatusCode(403,
                    new { message = "Only Participants can cancel enrolments." });

            var participant = await _db.Participants
                .FirstOrDefaultAsync(p => p.UserId == userId.Value);

            if (participant == null)
                return NotFound(
                    new { message = "Participant profile not found." });

            var enrolment = await _db.Enrolments
                .FirstOrDefaultAsync(e => e.EnrolId == id);

            if (enrolment == null)
                return NotFound(new { message = "Enrolment not found." });

            if (enrolment.ParticipantId != participant.ParticipantId)
                return StatusCode(403,
                    new { message = "You can only cancel your own enrolments." });

            bool hasResult = await _db.Results
                .AnyAsync(r => r.EnrolId == id);

            if (hasResult)
                return Conflict(
                    new { message = "An enrolment with a result cannot be cancelled." });

            _db.Enrolments.Remove(enrolment);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Enrolment cancelled successfully." });
        }// end of method

    }// end of class

}// end of namespace

