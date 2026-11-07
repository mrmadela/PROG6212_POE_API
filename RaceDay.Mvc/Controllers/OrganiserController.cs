using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;
using System.Text.Json;
using RaceDay.Mvc.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts;
using RaceDay.Mvc.Services;
using System.Net.Http;


namespace RaceDay.Mvc.Controllers
{// start of namespace

    [RoleAuthorize("Organiser")]
    public class OrganiserController : Controller
    {// start of class

        private readonly IApiService _apiService;

        public OrganiserController(IApiService apiService)
        {// start of constructor

            _apiService = apiService;

        }// end of constructor

        // Displays the organiser dashboard.
        public IActionResult Dashboard()
        {// start of Dashboard

            return View();

        }// end of Dashboard

        // Retrieves events belonging to the logged-in organiser.
        public async Task<IActionResult> Events()
        {// start of Events

            try
            {
                var events = await _apiService.GetAsync<List<EventDto>>(
                    "api/events/mine");

                return View(events ?? new List<EventDto>());
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Unable to load your events. Please try again.";

                return View(new List<EventDto>());
            }

        }// end of Events


        // Displays the create event form.
        [HttpGet]
        public IActionResult CreateEvent()
        {// start of CreateEvent GET

            return View(new EventViewModel
            {
                Date = DateTime.Today
            });

        }// end of CreateEvent GET

        // Sends the new event details to the API.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(EventViewModel model)
        {// start of CreateEvent POST

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var parameters = new Dictionary<string, string?>
                {
                    ["eventName"] = model.EventName,
                    ["eventDescription"] = model.EventDescription,
                    ["date"] = model.Date.ToString("O", CultureInfo.InvariantCulture),
                    ["location"] = model.Location,
                    ["distanceKm"] = model.DistanceKm.ToString(CultureInfo.InvariantCulture),
                    ["eventType"] = model.EventType
                };

                string endpoint = QueryHelpers.AddQueryString(
                    "api/events",
                    parameters);

                var request = new CreateEventRequest
                {
                    EventName = model.EventName,
                    EventDescription = model.EventDescription,
                    Date = model.Date,
                    Location = model.Location,
                    DistanceKm = model.DistanceKm,
                    EventType = model.EventType
                };

                var result = await _apiService.PostAsync<CreateEventRequest, EventDto>(
                    endpoint,
                    request);

                if (result == null)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "The API did not return the created event.");

                    return View(model);
                }

                TempData["SuccessMessage"] = "Event created successfully.";

                return RedirectToAction(nameof(Events));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the event. Check your details and try again.");

                return View(model);
            }

        }// end of CreateEvent POST


        // Loads an existing event into the edit form.
        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {// start of EditEvent GET

            try
            {
                var item = await _apiService.GetAsync<EventDto>(
                    $"api/events/{id}");

                if (item == null)
                {
                    return NotFound();
                }

                var model = new EventViewModel
                {
                    EventId = item.EventId,
                    EventName = item.EventName,
                    EventDescription = item.EventDescription,
                    Date = item.Date,
                    Location = item.Location,
                    DistanceKm = item.DistanceKm,
                    EventType = item.EventType
                };

                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Unable to load the event.";
                return RedirectToAction(nameof(Events));
            }

        }// end of EditEvent GET

        // Sends updated event details to the API.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(
            int id,
            EventViewModel model)
        {// start of EditEvent POST

            if (id != model.EventId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var parameters = new Dictionary<string, string?>
                {
                    ["eventName"] = model.EventName,
                    ["eventDescription"] = model.EventDescription,
                    ["date"] = model.Date.ToString("O", CultureInfo.InvariantCulture),
                    ["location"] = model.Location,
                    ["distanceKm"] = model.DistanceKm.ToString(CultureInfo.InvariantCulture),
                    ["eventType"] = model.EventType
                };

                string endpoint = QueryHelpers.AddQueryString(
                    $"api/events/{id}",
                    parameters);

                var request = new UpdateEventRequest
                {
                    EventName = model.EventName,
                    EventDescription = model.EventDescription,
                    Date = model.Date,
                    Location = model.Location,
                    DistanceKm = model.DistanceKm,
                    EventType = model.EventType
                };

                await _apiService.PutAsync<UpdateEventRequest, JsonElement>(
                    endpoint,
                    request);

                TempData["SuccessMessage"] = "Event updated successfully.";

                return RedirectToAction(nameof(Events));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the event. Please try again.");

                return View(model);
            }

        }// end of EditEvent POST


        // Displays a confirmation page before deleting an event.
        [HttpGet]
        public async Task<IActionResult> DeleteEvent(int id)
        {// start of DeleteEvent GET

            try
            {
                var item = await _apiService.GetAsync<EventDto>(
                    $"api/events/{id}");

                if (item == null)
                {
                    return NotFound();
                }

                return View(item);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Unable to find the event.";
                return RedirectToAction(nameof(Events));
            }

        }// end of DeleteEvent GET

        // Deletes the event through the API after confirmation.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEventConfirmed(int id)
        {// start of DeleteEventConfirmed

            try
            {
                bool deleted = await _apiService.DeleteAsync(
                    $"api/events/{id}");

                if (!deleted)
                {
                    TempData["ErrorMessage"] =
                        "The event could not be deleted. It may have categories.";

                    return RedirectToAction(nameof(Events));
                }

                TempData["SuccessMessage"] = "Event deleted successfully.";
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Unable to delete the event. Please try again.";
            }

            return RedirectToAction(nameof(Events));

        }// end of DeleteEventConfirmed




        // GET: Organiser/Categories/5
        [HttpGet]
        public async Task<IActionResult> Categories(int eventId)
        {
            // Make the event ID available to the view immediately.
            ViewBag.EventId = eventId;

            try
            {
                var categories = await _apiService.GetAsync<List<CategoryDto>>(
                    $"api/events/{eventId}/categories");

                return View(categories ?? new List<CategoryDto>());
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Unable to retrieve event categories.";

                return View(new List<CategoryDto>());
            }
        }

        // GET: Organiser/CreateCategory/5
        [HttpGet]
        public IActionResult CreateCategory(int eventId)
        {
            var model = new CategoryViewModel
            {
                EventId = eventId
            };

            return View(model);
        }

        // POST: Organiser/CreateCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(
            CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var request = new CategoryDto
                {
                    CategoryName = model.CategoryName.Trim(),
                    Description = model.Description,
                    CategoryType = model.CategoryType,
                    AgeGroup = model.CategoryType == "Age"
                        ? model.AgeGroup?.Trim()
                        : null,
                    DistanceKm = model.CategoryType == "Distance"
                        ? model.DistanceKm
                        : null,
                    EventId = model.EventId
                };

                var parameters = new Dictionary<string, string?>
                {
                    ["categoryName"] = request.CategoryName,
                    ["description"] = request.Description,
                    ["categoryType"] = request.CategoryType,
                    ["ageGroup"] = request.AgeGroup,
                    ["distanceKm"] = request.DistanceKm?.ToString(
                        CultureInfo.InvariantCulture)
                };

                string endpoint = QueryHelpers.AddQueryString(
                    $"api/events/{model.EventId}/categories",
                    parameters);

                var result = await _apiService.PostAsync<CategoryDto, CategoryDto>(
                    endpoint,
                    request);

                if (result == null)
                {
                    TempData["ErrorMessage"] = "Category creation failed.";
                    return View(model);
                }

                TempData["Success"] = "Category created successfully.";

                return RedirectToAction(
                    nameof(Categories),
                    new { eventId = model.EventId });
            }
            catch (HttpRequestException)
            {
                TempData["Error"] =
                    "The API could not create the category. Check the category details.";

                return View(model);
            }
        }

        // GET: Organiser/EditCategory/5
        [HttpGet]
        public async Task<IActionResult> EditCategory(
            int id,
            int eventId)
        {
            try
            {
                var categories = await _apiService.GetAsync<List<CategoryDto>>(
                    $"api/events/{eventId}/categories");

                var category = categories?.FirstOrDefault(
                    c => c.CategoryId == id);

                if (category == null)
                {
                    TempData["Error"] = "Category not found.";

                    return RedirectToAction(
                        nameof(Categories),
                        new { eventId });
                }

                var model = new CategoryViewModel
                {
                    CategoryId = category.CategoryId,
                    EventId = category.EventId,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    CategoryType = category.CategoryType,
                    AgeGroup = category.AgeGroup,
                    DistanceKm = category.DistanceKm
                };

                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to retrieve the category.";

                return RedirectToAction(
                    nameof(Categories),
                    new { eventId });
            }
        }

        // POST: Organiser/EditCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(
            CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var request = new CategoryDto
                {
                    CategoryId = model.CategoryId,
                    EventId = model.EventId,
                    CategoryName = model.CategoryName.Trim(),
                    Description = model.Description,
                    CategoryType = model.CategoryType,
                    AgeGroup = model.CategoryType == "Age"
                        ? model.AgeGroup?.Trim()
                        : null,
                    DistanceKm = model.CategoryType == "Distance"
                        ? model.DistanceKm
                        : null
                };

                var parameters = new Dictionary<string, string?>
                {
                    ["categoryName"] = request.CategoryName,
                    ["description"] = request.Description,
                    ["categoryType"] = request.CategoryType,
                    ["ageGroup"] = request.AgeGroup,
                    ["distanceKm"] = request.DistanceKm?.ToString(
                        CultureInfo.InvariantCulture)
                };

                string endpoint = QueryHelpers.AddQueryString(
                    $"api/categories/{model.CategoryId}",
                    parameters);

                await _apiService.PutAsync<CategoryDto, JsonElement>(
                    endpoint,
                    request);

                TempData["Success"] = "Category updated successfully.";

                return RedirectToAction(
                    nameof(Categories),
                    new { eventId = model.EventId });
            }
            catch (HttpRequestException)
            {
                TempData["Error"] =
                    "Unable to update the category. Check the details.";

                return View(model);
            }
        }

        // GET: Organiser/DeleteCategory/5
        [HttpGet]
        public async Task<IActionResult> DeleteCategory(
            int id,
            int eventId)
        {
            try
            {
                var categories = await _apiService.GetAsync<List<CategoryDto>>(
                    $"api/events/{eventId}/categories");

                var category = categories?.FirstOrDefault(
                    c => c.CategoryId == id);

                if (category == null)
                {
                    TempData["Error"] = "Category not found.";

                    return RedirectToAction(
                        nameof(Categories),
                        new { eventId });
                }

                return View(category);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to retrieve the category.";

                return RedirectToAction(
                    nameof(Categories),
                    new { eventId });
            }
        }

        // POST: Organiser/DeleteCategoryConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategoryConfirmed(
            int id,
            int eventId)
        {
            try
            {
                bool deleted = await _apiService.DeleteAsync(
                    $"api/categories/{id}");

                if (deleted)
                {
                    TempData["Success"] = "Category deleted successfully.";
                }
                else
                {
                    TempData["Error"] =
                        "Category could not be deleted. Participants may be enrolled.";
                }
            }
            catch (HttpRequestException)
            {
                TempData["Error"] =
                    "Unable to delete the category. It may have existing enrolments.";
            }

            return RedirectToAction(
                nameof(Categories),
                new { eventId });
        }




        // START OF EVENT ENROLMENTS ACTION
        [HttpGet]
        public async Task<IActionResult> Enrolments(int eventId)
        {
            ViewBag.EventId = eventId;

            try
            {
                var enrolments = await _apiService.GetAsync<List<EnrolmentDto>>(
                    $"api/enrolments/event/{eventId}");

                return View(enrolments ?? new List<EnrolmentDto>());
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Unable to retrieve enrolments for this event.";

                return View(new List<EnrolmentDto>());
            }
        }
        // END OF EVENT ENROLMENTS ACTION



        // START OF EVENT RESULTS ACTION
        [HttpGet]
        public async Task<IActionResult> Results(int eventId)
        {
            ViewBag.EventId = eventId;

            try
            {
                var results = await _apiService.GetAsync<List<ResultDto>>(
                    $"api/results/event/{eventId}");

                return View(results ?? new List<ResultDto>());
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Unable to retrieve results for this event.";

                return View(new List<ResultDto>());
            }
        }
        // END OF EVENT RESULTS ACTION

        // START OF CREATE RESULT GET
        [HttpGet]
        public IActionResult CreateResult(int eventId, int enrolId)
        {
            var model = new ResultViewModel
            {
                EventId = eventId,
                EnrolId = enrolId
            };

            return View(model);
        }
        // END OF CREATE RESULT GET

        // START OF CREATE RESULT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateResult(
            ResultViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var request = new ResultDto
                {
                    EnrolId = model.EnrolId,
                    Duration = model.Duration,
                    AverageSpeed = model.AverageSpeed,
                    Position = model.Position
                };

                var parameters = new Dictionary<string, string?>
                {
                    ["enrolId"] = model.EnrolId.ToString(),

                    ["duration"] = model.Duration?.ToString(
                        "c", CultureInfo.InvariantCulture),

                    ["averageSpeed"] = model.AverageSpeed?.ToString(
                        CultureInfo.InvariantCulture),

                    ["position"] = model.Position?.ToString()
                };

                string endpoint = QueryHelpers.AddQueryString(
                    "api/results",
                    parameters);

                var result = await _apiService.PostAsync<ResultDto, ResultDto>(
                    endpoint,
                    request);

                if (result == null)
                {
                    TempData["ErrorMessage"] =
                        "Result creation failed.";

                    return View(model);
                }

                TempData["SuccessMessage"] =
                    "Result recorded successfully.";

                return RedirectToAction(
                    nameof(Results),
                    new { eventId = model.EventId });
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Unable to record the result. Check the enrolment and result details.";

                return View(model);
            }
        }
        // END OF CREATE RESULT POST

    }// end of class

}// end of namespace
