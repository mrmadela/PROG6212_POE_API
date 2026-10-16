using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{
    
    [ApiController]
    [Route("api")]
    public class CategoriesController : ControllerBase
    {// start of class
        private readonly RaceDayDbContext _db;

        
        public CategoriesController(RaceDayDbContext db)
        {
            _db = db;
        }



        private async Task<Organiser?> GetCurrentOrganiser()
        {// start of method
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return null;

            return await _db.Organisers
                .FirstOrDefaultAsync(o => o.UserId == userId.Value);
        }// end of method


        // GET: /api/events/5/categories
        [HttpGet("events/{eventId:int}/categories")]
        public async Task<IActionResult> GetCategories(int eventId)
        {// start of method
            bool eventExists = await _db.Events
                .AnyAsync(e => e.EventId == eventId);

            if (!eventExists)
                return NotFound(new { message = "Event not found." });

            var categories = await _db.Categories
                .AsNoTracking()
                .Where(c => c.EventId == eventId)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    CategoryType = c.CategoryType,
                    AgeGroup = c.AgeGroup,
                    DistanceKm = c.DistanceKm,
                    EventId = c.EventId
                })
                .ToListAsync();

            return Ok(categories);
        }// end of method



        // POST: /api/events/5/categories
        [HttpPost("events/{eventId:int}/categories")]
        public async Task<IActionResult> CreateCategory(
            [FromRoute] int eventId,
            [FromQuery] string categoryName,
            [FromQuery] string? description,
            [FromQuery] string categoryType,
            [FromQuery] string? ageGroup,
            [FromQuery] decimal? distanceKm)
        {// start of method
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized(new { message = "Please log in." });

            if (role != "Organiser")
                return StatusCode(403,
                    new { message = "Only Organisers can create categories." });

            if (string.IsNullOrWhiteSpace(categoryName))
                return BadRequest(new { message = "Category name is required." });

            var organiser = await GetCurrentOrganiser();

            if (organiser == null)
                return NotFound(new { message = "Organiser profile not found." });

            var eventItem = await _db.Events
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            if (eventItem.OrgId != organiser.OrgId)
                return StatusCode(403,
                    new { message = "You can only manage categories for your own events." });

            bool validAgeCategory =
                categoryType == "Age" &&
                !string.IsNullOrWhiteSpace(ageGroup) &&
                distanceKm == null;

            bool validDistanceCategory =
                categoryType == "Distance" &&
                distanceKm.HasValue &&
                distanceKm.Value > 0 &&
                string.IsNullOrWhiteSpace(ageGroup);

            if (!validAgeCategory && !validDistanceCategory)
                return BadRequest(new
                {
                    message = "Age categories require AgeGroup only. Distance categories require a positive DistanceKm only."
                });

            var category = new Category
            {
                CategoryName = categoryName.Trim(),
                Description = description,
                CategoryType = categoryType,
                AgeGroup = ageGroup,
                DistanceKm = distanceKm,
                EventId = eventId
            };

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            var response = new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                CategoryType = category.CategoryType,
                AgeGroup = category.AgeGroup,
                DistanceKm = category.DistanceKm,
                EventId = category.EventId
            };

            return CreatedAtAction(
                nameof(GetCategories),
                new { eventId },
                response);
        }// end of method


        // PUT: /api/categories/5
        [HttpPut("categories/{id:int}")]
        public async Task<IActionResult> UpdateCategory(
            [FromRoute] int id,
            [FromQuery] string categoryName,
            [FromQuery] string? description,
            [FromQuery] string categoryType,
            [FromQuery] string? ageGroup,
            [FromQuery] decimal? distanceKm)
        {// start of method
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized(new { message = "Please log in." });

            if (role != "Organiser")
                return StatusCode(403,
                    new { message = "Only Organisers can update categories." });

            if (string.IsNullOrWhiteSpace(categoryName))
                return BadRequest(new { message = "Category name is required." });

            var organiser = await GetCurrentOrganiser();

            if (organiser == null)
                return NotFound(new { message = "Organiser profile not found." });

            var category = await _db.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
                return NotFound(new { message = "Category not found." });

            var eventItem = await _db.Events
                .FirstOrDefaultAsync(e => e.EventId == category.EventId);

            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            if (eventItem.OrgId != organiser.OrgId)
                return StatusCode(403,
                    new { message = "You can only update your own event categories." });

            bool validAgeCategory =
                categoryType == "Age" &&
                !string.IsNullOrWhiteSpace(ageGroup) &&
                distanceKm == null;

            bool validDistanceCategory =
                categoryType == "Distance" &&
                distanceKm.HasValue &&
                distanceKm.Value > 0 &&
                string.IsNullOrWhiteSpace(ageGroup);

            if (!validAgeCategory && !validDistanceCategory)
                return BadRequest(new
                {
                    message = "Age categories require AgeGroup only. Distance categories require a positive DistanceKm only."
                });

            category.CategoryName = categoryName.Trim();
            category.Description = description;
            category.CategoryType = categoryType;
            category.AgeGroup = ageGroup;
            category.DistanceKm = distanceKm;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Category updated successfully." });
        }// end of method


        // DELETE: /api/categories/5
        [HttpDelete("categories/{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {// start of method
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return Unauthorized(new { message = "Please log in." });

            if (role != "Organiser")
                return StatusCode(403,
                    new { message = "Only Organisers can delete categories." });

            var organiser = await GetCurrentOrganiser();

            if (organiser == null)
                return NotFound(
                    new { message = "Organiser profile not found." });

            var category = await _db.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
                return NotFound(new { message = "Category not found." });

            var eventItem = await _db.Events
                .FirstOrDefaultAsync(e => e.EventId == category.EventId);

            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            if (eventItem.OrgId != organiser.OrgId)
                return StatusCode(403,
                    new { message = "You can only delete your own event categories." });

            bool hasEnrolments = await _db.Enrolments
                .AnyAsync(e => e.CategoryId == id);

            if (hasEnrolments)
                return Conflict(new
                {
                    message = "This category cannot be deleted because participants are enrolled."
                });

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Category deleted successfully." });
        }// end of method

    }// end of class

}// end of namespace

