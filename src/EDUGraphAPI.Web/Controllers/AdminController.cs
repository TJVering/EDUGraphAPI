﻿using EDUGraphAPI.Utils;
using EDUGraphAPI.Web.Infrastructure;
using EDUGraphAPI.Web.Services;
using EDUGraphAPI.Web.Services.GraphClients;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AAD = Microsoft.Azure.ActiveDirectory.GraphClient;

namespace EDUGraphAPI.Web.Controllers
{
    [EduAuthorize(Roles = "Admin"), HandleAdalException]
    public class AdminController : Controller
    {
        static readonly string StateKey = typeof(AdminController).Name + "State";

        private ApplicationService applicationService;

        public AdminController(ApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        //
        // GET: /Admin/Index
        public async Task<ActionResult> Index()
        {
            var adminContext = await applicationService.GetAdminContextAsync();
            return View(adminContext);
        }

        //
        // POST: /Admin/Consent
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Consent()
        {
            // generate a random value to identify the request
            var stateMarker = Guid.NewGuid().ToString();
            TempData[StateKey] = stateMarker;

            //create an OAuth2 request, using the web app as the client.
            //this will trigger a consent flow that will provision the app in the target tenant
            var redirectUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Admin/ProcessCode";
            var authorizationUrl = AuthorizationHelper.GetUrl(redirectUrl, stateMarker, Constants.Resources.AADGraph, AuthorizationHelper.Prompt.AdminConsent);

            // send the admin to consent
            return new RedirectResult(authorizationUrl);
        }

        //
        // GET: /Admin/ProcessCode
        public async Task<ActionResult> ProcessCode(string code, string error, string error_description, string resource, string state)
        {
            if (TempData[StateKey] as string != state)
            {
                TempData["Error"] = "Invalid operation. Please try again";
                return RedirectToAction("Index");
            }

            // Get the tenant
            var authResult = await AuthenticationHelper.GetAuthenticationResultAsync(code);
            var activeDirectoryClient = authResult.CreateActiveDirectoryClient();
            var graphClient = new AADGraphClient(activeDirectoryClient);
            var tenant = await graphClient.GetTenantAsync(authResult.TenantId);

            // Create (or update) an organization, and make it as AdminConsented
            await applicationService.CreateOrUpdateOrganizationAsync(tenant, true);

            TempData["Message"] = "Admin consented successfully!";
            return RedirectToAction("Index");
        }

        //
        // POST: /Admin/Unconsent
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Unconsent()
        {
            var client = await AuthenticationHelper.GetActiveDirectoryClientAsync(Permissions.Delegated);
            var servicePrincipal = await client.ServicePrincipals
               .Where(i => i.AppId == Constants.AADClientId)
               .ExecuteSingleAsync();
            if (servicePrincipal != null)
                await servicePrincipal.DeleteAsync();

            var adminContext = await applicationService.GetAdminContextAsync();
            if (adminContext.Organization != null)
            {
                var tenantId = adminContext.Organization.TenantId;
                await applicationService.UpdateOrganizationAsync(tenantId, false);
                await applicationService.UnlinkAllAccounts(tenantId);
            }

            TempData["Message"] = "Admin unconsented successfully!";
            return RedirectToAction("Index");
        }

        //
        // GET: /Admin/LinkedAccounts
        public async Task<ActionResult> LinkedAccounts()
        {
            var adminContext = await applicationService.GetAdminContextAsync();
            var users = await applicationService.GetLinkedUsers(i => i.OrganizationId == adminContext.Organization.Id);
            return View(users);
        }

        //
        // GET: /Admin/UnlinkAccounts
        public async Task<ActionResult> UnlinkAccounts(string id)
        {
            var user = await applicationService.GetUserAsync(id);
            return View(user);
        }

        //
        // POST: /Admin/UnlinkAccounts
        [HttpPost, ValidateAntiForgeryToken, ActionName("UnlinkAccounts")]
        public async Task<ActionResult> UnlinkAccountsPost(string id)
        {
            await applicationService.UnlinkAccountsAsync(id);
            return RedirectToAction("LinkedAccounts");
        }

        //
        // POST: /Admin/AddAppRoleAssignments
        [HttpPost]
        public async Task<ActionResult> AddAppRoleAssignments()
        {
            var client = await AuthenticationHelper.GetActiveDirectoryClientAsync(Permissions.Delegated);

            var servicePrincipal = await client.ServicePrincipals
               .Where(i => i.AppId == Constants.AADClientId)
               .ExecuteSingleAsync();
            if (servicePrincipal == null)
            {
                TempData["Error"] = "Could not found the service principal. Please provdie the admin consent.";
                return RedirectToAction("Index");
            }

            int count = 0;
            var tasks = new List<Task>();
            var resourceId = new Guid(servicePrincipal.ObjectId);
            var users = await client.Users
                .Expand(i => i.AppRoleAssignments)
                .ExecuteAllAsync();

            foreach (var user in users)
            {
                var task = Task.Run(async () =>
                {
                    if (await user.AppRoleAssignments.AnyAsync(i => i.ResourceId == resourceId)) return;

                    // https://github.com/microsoftgraph/microsoft-graph-docs/blob/master/api-reference/beta/resources/approleassignment.md
                    var appRoleAssignment = new AAD.AppRoleAssignment
                    {
                        CreationTimestamp = DateTime.UtcNow,
                        //Id = Guid.Empty,
                        PrincipalDisplayName = user.DisplayName,
                        PrincipalId = new Guid(user.ObjectId),
                        PrincipalType = "User",
                        ResourceId = resourceId,
                        ResourceDisplayName = servicePrincipal.DisplayName
                    };
                    var userFetcher = client.Users.GetByObjectId(user.ObjectId);
                    try
                    {
                        await userFetcher.AppRoleAssignments.AddAppRoleAssignmentAsync(appRoleAssignment);
                    }
                    catch { }
                    Interlocked.Increment(ref count);
                });
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());

            TempData["Message"] = count > 0
                ? $"User access was successfully enabled for {count} user(s)."
                : "User access was enabled for all users.";
            return RedirectToAction("Index");
        }
    }
}