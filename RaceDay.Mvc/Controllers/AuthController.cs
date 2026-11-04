using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts;
using RaceDay.Mvc.Models.ViewModels;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{// start of namespace
    
    public class AuthController : Controller
    {// start of class
        private readonly AuthApiService _authApiService;

        public AuthController(AuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        
        [HttpGet]
        public IActionResult Login()
        {// start of Login GET method
            return View();
        }
        // end of Login GET method

        // start of Login POST method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var request = new LoginRequest
                {
                    Email = model.Email,
                    Password = model.Password
                };

                AuthResponse? response =
                    await _authApiService.LoginAsync(request);

                if (response == null)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Unable to log in. Please try again.");

                    return View(model);
                }

                HttpContext.Session.SetInt32(
                    "UserId", response.UserId);

                HttpContext.Session.SetString(
                    "Role", response.Role);

                HttpContext.Session.SetString(
                    "Email", response.Email);

                if (response.Role == "Organiser")
                {
                    return RedirectToAction(
                        "Dashboard", "Organiser");
                }

                return RedirectToAction(
                    "Index", "Participant");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Login failed. Check your email and password.");

                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "A temporary error occurred. Please try again.");

                return View(model);
            }
        }
        // end of Login POST method

        // start of Register GET method
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        // end of Register GET method

        // start of Register POST method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var request = new RegisterRequest
                {
                    Email = model.Email,
                    Password = model.Password,
                    Role = model.Role,
                    FullName = model.FullName,
                    PhoneNum = model.PhoneNum,
                    IdNum = model.IdNum,
                    Address = model.Address
                };

                AuthResponse? response =
                    await _authApiService.RegisterAsync(request);

                if (response != null)
                {
                    TempData["SuccessMessage"] =
                        "Registration successful. Please log in.";

                    return RedirectToAction("Login");
                }

                ModelState.AddModelError(
                    string.Empty,
                    "Registration could not be completed.");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "A temporary error occurred. Please try again.");
            }

            return View(model);
        }
        // end of Register POST method

        // start of Logout method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authApiService.LogoutAsync();

            return RedirectToAction("Login");
        }
        // end of Logout method

        // start of AccessDenied method
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        // end of AccessDenied method
    }// end of class

}// end of namespace
