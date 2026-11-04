using Microsoft.AspNetCore.Http;

namespace RaceDay.Mvc.Helpers
{// start of namespace
    // start of class
    public static class SessionHelper
    {
        public static int? GetUserId(ISession session)
        {
            return session.GetInt32("UserId");
        }

        public static string? GetRole(ISession session)
        {
            return session.GetString("Role");
        }

        public static bool IsLoggedIn(ISession session)
        {
            return GetUserId(session).HasValue &&
                   !string.IsNullOrWhiteSpace(GetRole(session));
        }
    }
    // end of class
}// end of namespace
