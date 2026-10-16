using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{// 
    
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {// start of class
        private readonly RaceDayDbContext _db;

        public EventsController(RaceDayDbContext db)
        {
            _db = db;
        }

        
        // GET: /api/events
        // Both Organisers and Participants can browse events.
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {// start of method
            var events = await _db.Events
                .AsNoTracking()
                .Select(e => new EventDto
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    EventDescription = e.EventDescription,
                    Date = e.Date,
                    Location = e.Location,
                    DistanceKm = e.DistanceKm,
                    EventType = e.EventType,
                    OrgId = e.OrgId
                })
                .ToListAsync();

            return Ok(events);
        }// end of method



        // GET: /api/events/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEvent(int id)
        {// start of method
            var item = await _db.Events
                .AsNoTracking()
                .Where(e => e.EventId == id)
                .Select(e => new EventDto
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    EventDescription = e.EventDescription,
                    Date = e.Date,
                    Location = e.Location,
                    DistanceKm = e.DistanceKm,
                    EventType = e.EventType,
                    OrgId = e.OrgId
                })
                .FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            return Ok(item);
        }// end of method



        // GET: /api/events/mine
        // Lists events belonging to the logged-in Organiser.
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyEvents()
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
                return NotFound(new { message = "Organiser profile not found." });

            var events = await _db.Events
                .AsNoTracking()
                .Where(e => e.OrgId == organiser.OrgId)
                .Select(e => new EventDto
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    EventDescription = e.EventDescription,
                    Date = e.Date,
                    Location = e.Location,
                    DistanceKm = e.DistanceKm,
                    EventType = e.EventType,
                    OrgId = e.OrgId
                })
                .ToListAsync();

            return Ok(events);
        }// end of method


        // POST: /api/events
        // Only an Organiser can create an event using URL query parameters.
        [HttpPost]
        public async Task<IActionResult> CreateEvent(
            [FromQuery] string eventName,
            [FromQuery] string? eventDescription,
            [FromQuery] DateTime date,
            [FromQuery] string location,
            [FromQuery] decimal distanceKm,
            [FromQuery] string eventType)
        {// start of method
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized(new { message = "Please log in." });

            if (role != "Organiser")
                return StatusCode(403,
                    new { message = "Only Organisers can create events." });

            if (string.IsNullOrWhiteSpace(eventName) ||
                string.IsNullOrWhiteSpace(location))
                return BadRequest(new
                {
                    message = "Event name and location are required."
                });

            if (distanceKm <= 0)
                return BadRequest(new
                {
                    message = "Distance must be greater than zero."
                });

            if (eventType != "Run" &&
                eventType != "Walk" &&
                eventType != "Cycle")
                return BadRequest(new
                {
                    message = "Event type must be Run, Walk, or Cycle."
                });

            var organiser = await _db.Organisers
                .FirstOrDefaultAsync(o => o.UserId == userId.Value);

            if (organiser == null)
                return NotFound(new
                {
                    message = "Organiser profile not found."
                });

            var newEvent = new Event
            {
                EventName = eventName.Trim(),
                EventDescription = eventDescription,
                Date = date,
                Location = location.Trim(),
                DistanceKm = distanceKm,
                EventType = eventType,
                OrgId = organiser.OrgId
            };

            _db.Events.Add(newEvent);
            await _db.SaveChangesAsync();

            var response = new EventDto
            {
                EventId = newEvent.EventId,
                EventName = newEvent.EventName,
                EventDescription = newEvent.EventDescription,
                Date = newEvent.Date,
                Location = newEvent.Location,
                DistanceKm = newEvent.DistanceKm,
                EventType = newEvent.EventType,
                OrgId = newEvent.OrgId
            };

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = newEvent.EventId },
                response);
        }// end of method


        // PUT: /api/events/5
        // Only the event's Organiser can update it.
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEvent(
            [FromRoute] int id,
            [FromQuery] string eventName,
            [FromQuery] string? eventDescription,
            [FromQuery] DateTime date,
            [FromQuery] string location,
            [FromQuery] decimal distanceKm,
            [FromQuery] string eventType)
        {// start of method
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized(new { message = "Please log in." });

            if (role != "Organiser")
                return StatusCode(403,
                    new { message = "Only Organisers can update events." });

            if (string.IsNullOrWhiteSpace(eventName) ||
                string.IsNullOrWhiteSpace(location))
                return BadRequest(new
                {
                    message = "Event name and location are required."
                });

            if (distanceKm <= 0)
                return BadRequest(new
                {
                    message = "Distance must be greater than zero."
                });

            if (eventType != "Run" &&
                eventType != "Walk" &&
                eventType != "Cycle")
                return BadRequest(new
                {
                    message = "Event type must be Run, Walk, or Cycle."
                });

            var organiser = await _db.Organisers
                .FirstOrDefaultAsync(o => o.UserId == userId.Value);

            if (organiser == null)
                return NotFound(new
                {
                    message = "Organiser profile not found."
                });

            var item = await _db.Events
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (item == null)
                return NotFound(new { message = "Event not found." });

            if (item.OrgId != organiser.OrgId)
                return StatusCode(403,
                    new { message = "You can only update your own events." });

            item.EventName = eventName.Trim();
            item.EventDescription = eventDescription;
            item.Date = date;
            item.Location = location.Trim();
            item.DistanceKm = distanceKm;
            item.EventType = eventType;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Event updated successfully."
            });
        }// end of method


        // DELETE: /api/events/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {// start of method
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized(new { message = "Please log in." });

            if (role != "Organiser")
                return StatusCode(403,
                    new { message = "Only Organisers can delete events." });

            var organiser = await _db.Organisers
                .FirstOrDefaultAsync(o => o.UserId == userId.Value);

            if (organiser == null)
                return NotFound(new { message = "Organiser profile not found." });

            var item = await _db.Events
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (item == null)
                return NotFound(new { message = "Event not found." });

            if (item.OrgId != organiser.OrgId)
                return StatusCode(403,
                    new { message = "You can only delete your own events." });

            bool hasCategories = await _db.Categories
                .AnyAsync(c => c.EventId == id);

            if (hasCategories)
                return Conflict(new
                {
                    message = "Delete the event's categories first."
                });

            _db.Events.Remove(item);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Event deleted successfully." });
        }// end of method

    }// end of class

}// end of namespace

