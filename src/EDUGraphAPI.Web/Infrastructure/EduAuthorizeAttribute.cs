using System.Web.Mvc;

namespace EDUGraphAPI.Web.Infrastructure
{
    public class EduAuthorizeAttribute: AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Account/Login");
        }
    }
}