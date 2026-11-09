
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Contracts;
using RaceDay.Mvc.Models.ViewModels;

namespace RaceDay.Mvc.Controllers
{// START OF NAMESPACE

    [RoleAuthorize("Participant")]
    public class ParticipantController : Controller
    {// START OF CLASS

        // START OF API SERVICE
        private readonly IApiService _apiService;

        public ParticipantController(IApiService apiService)
        {
            _apiService = apiService;
        }
        // END OF API SERVICE


        // START OF PARTICIPANT DASHBOARD
        public async Task<IActionResult> Index(
            string? searchTerm,
            string? eventType)
        {
            // Retrieve all events from the API.
            var events = await _apiService.GetAsync<List<EventDto>>(
                "api/events");

            // Handle an empty API response.
            events ??= new List<EventDto>();

            // Display only upcoming events.
            var upcomingEvents = events
                .Where(item => item.Date.Date >= DateTime.Today)
                .ToList();

            // Filter events by name or location.
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                upcomingEvents = upcomingEvents
                    .Where(item =>
                        item.EventName.Contains(
                            searchTerm,
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        item.Location.Contains(
                            searchTerm,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter events by event type.
            if (!string.IsNullOrWhiteSpace(eventType))
            {
                upcomingEvents = upcomingEvents
                    .Where(item =>
                        item.EventType.Equals(
                            eventType,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Pass the selected filters to the view.
            ViewBag.SearchTerm = searchTerm;
            ViewBag.EventType = eventType;

            return View(upcomingEvents);
        }
        // END OF PARTICIPANT DASHBOARD



        // START OF EVENT DETAILS ACTION
        public async Task<IActionResult> Details(int id)
        {
            // Retrieve the selected event.
            var eventItem = await _apiService.GetAsync<EventDto>(
                $"api/events/{id}");

            // Check whether the event exists.
            if (eventItem == null)
            {
                return NotFound();
            }

            // Retrieve categories belonging to the event.
            var categories = await _apiService.GetAsync<List<CategoryDto>>(
                $"api/events/{id}/categories");

            // Create the event details ViewModel.
            var viewModel = new EventDetailsViewModel
            {
                Event = eventItem,
                Categories = categories ?? new List<CategoryDto>()
            };

            return View(viewModel);
        }
        // END OF EVENT DETAILS ACTION



    }// END OF CLASS

}// END OF NAMESPACE
