using System.Web.Mvc;

namespace EDUGraphAPI.Web.Infrastructure
{
    // TODO: investigate why AuthorizeAttribute redirect the user to microsoft login page.
    public class EduAuthorizeAttribute: AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Account/Login");
        }
    }
}