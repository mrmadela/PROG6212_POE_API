
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{// start of namespace

    [RoleAuthorize("Participant")]
    public class ParticipantController : Controller
    {// start of class

        // Participant dashboard landing page.
        public IActionResult Index()
        {// start of Index

            return View();

        }// end of Index

    }// end of class

}// end of namespace
