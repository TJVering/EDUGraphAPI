﻿using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Web;
using System.Web.Mvc;

namespace EDUGraphAPI.Web.Infrastructure
{
    public class HandleAdalExceptionAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public static readonly string ChallengeImmediatelyTempDataKey = "ChallengeImmediately";

        public void OnException(ExceptionContext filterContext)
        {
            if (!(filterContext.Exception is AdalException)) return;

            var requestUrl = filterContext.HttpContext.Request.Url.ToString();
            var challengeImmediately = filterContext.Controller.TempData[ChallengeImmediatelyTempDataKey];
            if (challengeImmediately as bool? == true)
            {
                filterContext.HttpContext.GetOwinContext().Authentication.Challenge(
                   new AuthenticationProperties { RedirectUri = requestUrl },
                   OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            else
            {
                var redirectTo = "/Link/LoginO365Required?returnUrl=" + Uri.EscapeDataString(requestUrl);
                filterContext.Result = new RedirectResult(redirectTo);
            }
            filterContext.ExceptionHandled = true;

        }
    }
}