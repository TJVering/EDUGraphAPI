using EDUGraphAPI.Utils;
using EDUGraphAPI.Web.Infrastructure;
using EDUGraphAPI.Web.Services;
using EDUGraphAPI.Web.Services.GraphClients;
using Microsoft.Data.OData;
using System;
using System.Data.Services.Client;
using System.Threading.Tasks;
using System.Web.Mvc;
using AAD = Microsoft.Azure.ActiveDirectory.GraphClient;

namespace EDUGraphAPI.Web.Controllers
{
    [EduAuthorize, HandleAdalException]
    public class AdminController : Controller
    {
        static readonly string StateKey = typeof(AdminController).Name + "State";

        private ApplicationService applicationService;

        public AdminController(ApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        public async Task<ActionResult> Index()
        {
            var adminContext = await applicationService.GetAdminContextAsync();
            return View(adminContext);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SignUp()
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

            TempData["Message"] = "You signed up successfully!";
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> LinkedAccounts()
        {
            var users = await applicationService.GetLinkedUsers();
            return View(users);
        }

        public async Task<ActionResult> UnlinkAccounts(string id)
        {
            var user = await applicationService.GetUserAsync(id);
            return View(user);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("UnlinkAccounts")]
        public async Task<ActionResult> UnlinkAccountsPost(string id)
        {
            await applicationService.UnlinkAccountsAsync(id);
            return RedirectToAction("LinkedAccounts");
        }

        [HttpPost]
        public async Task<ActionResult> InstallApp()
        {
            var client = await AuthenticationHelper.GetActiveDirectoryClientAsync(Permissions.Delegated);

            var servicePrincipal = await client.ServicePrincipals
               .Where(i => i.AppId == Constants.AADClientId)
               .ExecuteSingleAsync();

            var resourceId = new Guid(servicePrincipal.ObjectId);

            int count = 0;
            var users = await client.Users.ExecuteAllAsync();
            foreach (var user in users)
            {
                var userFetcher = client.Users.GetByObjectId(user.ObjectId);

                var appRoleAssignment = await userFetcher.AppRoleAssignments
                    .Where(i => i.ResourceId == resourceId)
                    .ExecuteFirstOrDefaultAsync();
                if (appRoleAssignment != null) continue;

                // https://github.com/microsoftgraph/microsoft-graph-docs/blob/master/api-reference/beta/resources/approleassignment.md
                appRoleAssignment = new AAD.AppRoleAssignment
                {
                    CreationTimestamp = DateTime.UtcNow,
                    //Id = Guid.Empty,
                    PrincipalDisplayName = user.DisplayName,
                    PrincipalId = new Guid(user.ObjectId),
                    PrincipalType = "User",
                    ResourceId = resourceId,
                    ResourceDisplayName = servicePrincipal.DisplayName
                };

                try
                {
                    await userFetcher.AppRoleAssignments.AddAppRoleAssignmentAsync(appRoleAssignment);
                }
                catch (DataServiceRequestException ex)
                {
                    var isIdNullException = ex.InnerException is InvalidOperationException &&
                        ex.InnerException.Message.Contains("A null value was found for the property named 'id', which has the expected type 'Edm.Guid[Nullable=False]");
                    if (isIdNullException) { /* Actually, the AppRoleAssignment was added. We just ignore the exception. */ }
                    else throw;
                }
                catch (ODataErrorException)
                {
                    // Ignore this exception.
                }
                count++;
            }

            TempData["Message"] = count > 0
                ? $"The App was successfully installed for {count} user(s)."
                : "The App had been installed for all users.";
            return RedirectToAction("Index");
        }
    }
}