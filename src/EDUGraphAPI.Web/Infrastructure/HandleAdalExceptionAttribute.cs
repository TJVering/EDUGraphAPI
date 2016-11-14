using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Web.Mvc;

namespace EDUGraphAPI.Web.Infrastructure
{
    public class HandleAdalExceptionAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is AdalException)
            {                 
                var requestUrl = filterContext.HttpContext.Request.Url.ToString();
                var redirectTo = "/Link/LoginO365Required?returnUrl=" + Uri.EscapeDataString(requestUrl);
                filterContext.Result = new RedirectResult(redirectTo);
                filterContext.ExceptionHandled = true;
            }
        }
    }
}