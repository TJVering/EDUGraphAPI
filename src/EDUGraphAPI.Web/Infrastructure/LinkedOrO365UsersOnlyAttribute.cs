using EDUGraphAPI.Web.Services;
using System.Web;
using System.Web.Mvc;

namespace EDUGraphAPI.Web.Infrastructure
{
    public class LinkedOrO365UsersOnlyAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var applicationService = DependencyResolver.Current.GetService<ApplicationService>();
            var user = applicationService.GetUserContext();
            return user.AreAccountsLinked || user.IsO365Account;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new ViewResult { ViewName = "NoAccess" };
        }
    }
}