using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RaceDay.Mvc.Helpers;

namespace RaceDay.Mvc.Filters
{// start of namespace
    
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {// start of class
        private readonly string[] _allowedRoles;

        public RoleAuthorizeAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        // start of OnAuthorization method
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;

            if (!SessionHelper.IsLoggedIn(session))
            {
                context.Result = new RedirectToActionResult(
                    "Login", "Auth", null);

                return;
            }

            string? role = SessionHelper.GetRole(session);

            if (role == null ||
                !_allowedRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToActionResult(
                    "AccessDenied", "Auth", null);
            }
        }
        // end of OnAuthorization method
    }
    // end of class
}// end of namespace
