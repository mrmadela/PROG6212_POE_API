
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{// start of namespace

    [RoleAuthorize("Organiser")]
    public class OrganiserController : Controller
    {// start of class

        // Organiser dashboard landing page.
        public IActionResult Dashboard()
        {// start of Dashboard

            return View();

        }// end of Dashboard

    }// end of class

}// end of namespace
