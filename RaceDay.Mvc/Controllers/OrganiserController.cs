using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;
using System.Text.Json;
using RaceDay.Mvc.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts;
using RaceDay.Mvc.Services;


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



    }// end of class

}// end of namespace
