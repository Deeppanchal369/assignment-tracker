using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AssignmentTracker.Filters
{
    /// <summary>
    /// Simple session-based authorization filter.
    /// Pass "Teacher" or "Student" to restrict an action/controller to that role.
    /// </summary>
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string _requiredRole;

        public SessionAuthorizeAttribute(string requiredRole)
        {
            _requiredRole = requiredRole;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var role = session.GetString("UserRole");
            var userId = session.GetInt32("UserId");

            if (string.IsNullOrEmpty(role) || userId == null || role != _requiredRole)
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
