
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

    }// end of class

}// end of namespace
