﻿using EDUGraphAPI.Data;
using EDUGraphAPI.Utils;
using EDUGraphAPI.Web.Infrastructure;
using EDUGraphAPI.Web.Models;
using EDUGraphAPI.Web.Properties;
using EDUGraphAPI.Web.Services;
using EDUGraphAPI.Web.Services.GraphClients;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EDUGraphAPI.Web.Controllers
{
    [EduAuthorize, HandleAdalException]
    public class LinkController : Controller
    {
        static readonly string StateKey = typeof(LinkController).Name + "State";

        private ApplicationService applicationService;
        private ApplicationSignInManager signInManager;
        private ApplicationUserManager userManager;

        public LinkController(ApplicationService applicationService, ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            this.applicationService = applicationService;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<ActionResult> Index()
        {
            var userContext = await applicationService.GetUserContext();
            return View(userContext);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LoginO365()
        {
            var state = Guid.NewGuid().ToString();
            TempData[StateKey] = state;

            var redirectUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("ProcessCode");
            var authorizationUrl = AuthorizationHelper.GetUrl(redirectUrl, state, Constants.Resources.MSGraph, AuthorizationHelper.Prompt.Login);
            return new RedirectResult(authorizationUrl);
        }

        public async Task<ActionResult> ProcessCode(string code, string error, string error_description, string resource, string state)
        {
            if (TempData[StateKey] as string != state)
            {
                TempData["Error"] = "Invalid operation. Please try again";
                return RedirectToAction("Index");
            }

            var authResult = await AuthenticationHelper.GetAuthenticationResultAsync(code);
            var tenantId = authResult.TenantId;
            var graphServiceClient = authResult.CreateGraphServiceClient();

            IGraphClient graphClient = new MSGraphClient(graphServiceClient);
            var user = await graphClient.GetCurrentUserAsync();
            var tenant = await graphClient.GetTenantAsync(tenantId);

            var isAccountLinked = await applicationService.IsO365AccountLinkedAsync(user.Id);
            if (isAccountLinked)
            {
                TempData["Error"] = $"Failed to link accounts. The Office 365 account '{user.UserPrincipalName}' is already linked to another local account.";
                return RedirectToAction("Index");
            }

            // Link the AAD User with local user.
            var localUser = await applicationService.GetCurrentUserAsync();
            await applicationService.UpdateLocalUserAsync(localUser, user, tenant);

            // Re-sign in user. Required claims (roles, tenent id and user object id) will be added to current user's identity.
            await signInManager.SignInAsync(localUser, isPersistent: false, rememberBrowser: false);

            TempData["Message"] = Resources.LinkO365AccountSuccess;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LoginLocal()
        {
            return View();
        }

        [HttpPost, ActionName("LoginLocal"), ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginLocalPost(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var localUser = userManager.FindByEmail(model.Email);
            if (localUser.O365UserId.IsNotNullAndEmpty())
            {
                ModelState.AddModelError("Email", "The local account has already been linked to another Office 365 account.");
                return View(model);
            }
            if (!await userManager.CheckPasswordAsync(localUser, model.Password))
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            var tenantId = User.GetTenantId();
            var activeDirectoryClient = await AuthenticationHelper.GetActiveDirectoryClientAsync(Permissions.Delegated);

            IGraphClient graphClient = new AADGraphClient(activeDirectoryClient);
            var user = await graphClient.GetCurrentUserAsync();
            var tenant = await graphClient.GetTenantAsync(tenantId);

            await applicationService.UpdateLocalUserAsync(localUser, user, tenant);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> CreateLocalAccount()
        {
            var client = await AuthenticationHelper.GetActiveDirectoryClientAsync(Permissions.Delegated);
            var aadUser = await client.Me.ExecuteAsync();

            var viewModel = new EducationRegisterViewModel
            {
                FirstName = aadUser.GivenName,
                LastName = aadUser.Surname,
                Email = aadUser.UserPrincipalName
            };
            return View(viewModel);
        }

        [HttpPost, ActionName("CreateLocalAccount"), ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateLocalAccountPost(EducationRegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Create a new local user
            var localUser = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                FavoriteColor = model.FavoriteColor
            };
            var result = await userManager.CreateAsync(localUser, model.Password);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return View(model);
            }

            // Update the local user
            var tenantId = User.GetTenantId();
            var activeDirectoryClient = await AuthenticationHelper.GetActiveDirectoryClientAsync(Permissions.Delegated);

            IGraphClient graphClient = new AADGraphClient(activeDirectoryClient);
            var user = await graphClient.GetCurrentUserAsync();
            var tenant = await graphClient.GetTenantAsync(tenantId);

            user.GivenName = model.FirstName;
            user.Surname = model.LastName;
            await applicationService.UpdateLocalUserAsync(localUser, user, tenant);

            //
            return RedirectToAction("Index");
        }

        public ActionResult LoginO365Required(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult ReLoginO365(string returnUrl)
        {
            HttpContext.GetOwinContext().Authentication.Challenge(
                 new AuthenticationProperties { RedirectUri = returnUrl },
                 OpenIdConnectAuthenticationDefaults.AuthenticationType);
            return null;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}