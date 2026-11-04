
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Mvc.Services
{// start of namespace

    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {// start of class

        private readonly string _requiredRole;

        public RoleAuthorizeAttribute(string requiredRole)
        {// start of constructor
            _requiredRole = requiredRole;
        }// end of constructor

        public void OnAuthorization(AuthorizationFilterContext context)
        {// start of OnAuthorization

            var session = context.HttpContext.Session;

            var userId = session.GetInt32("UserId");
            var userRole = session.GetString("Role");

            // Redirect users who have not logged in.
            if (userId == null || string.IsNullOrWhiteSpace(userRole))
            {
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Auth",
                    null);

                return;
            }

            // Only allow users with the required role.
            if (!string.Equals(
                userRole,
                _requiredRole,
                StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToActionResult(
                    "AccessDenied",
                    "Auth",
                    null);
            }

        }// end of OnAuthorization

    }// end of class

}// end of namespace
