using EDUGraphAPI.Data;
using EDUGraphAPI.DataSync;
using EDUGraphAPI.DifferentialQuery;
using EDUGraphAPI.Models;
using EDUGraphAPI.Utils;
using EDUGraphAPI.Web.Infrastructure;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Graph = Microsoft.Graph;

namespace EDUGraphAPI.Web.Controllers
{
    [EduAuthorize, HandleAdalException]
    public class TestController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var client = await AuthenticationHelper.GetActiveDirectoryClientAsync(Permissions.Delegated);

            var tenant = await client.TenantDetails
                .Where(i => i.ObjectId == User.GetTenantId())
                .ExecuteSingleAsync();

            var servicePrincipals = await client.ServicePrincipals
               .Where(i => i.AppId == Constants.AADClientId)
               .ExecuteSingleAsync();

            var me = await client.Me.ExecuteAsync();
            return Content(me.DisplayName);
        }

        public async Task<ActionResult> CreateUser(
            string firstName = "Tyler", string lastName = "Lu",
            string tenantId = "64446b5c-6d85-4d16-9ff2-94eddc0c2439")
        {

            var token = await AuthenticationHelper.GetAccessTokenAsync(Constants.Resources.AADGraph, Permissions.Application);

            var client = await AuthenticationHelper.GetGraphServiceClientAsync(Permissions.Application);

            var organizations = await client.Organization.Request()
                .Filter($"id eq '{tenantId}'")
                .Top(1)
                .GetAsync();
            var organization = organizations.CurrentPage.FirstOrDefault();
            var organizationDomain = organization.VerifiedDomains.Select(i => i.Name).FirstOrDefault();

            var mailNickname = firstName.ToLower() + "." + lastName.ToLower();
            // https://msdn.microsoft.com/en-us/library/azure/ad/graph/api/users-operations#CreateUser
            var user = new Graph.User
            {
                AccountEnabled = true,
                GivenName = firstName,
                Surname = lastName,
                DisplayName = firstName + " " + lastName,
                MailNickname = mailNickname,
                UserPrincipalName = mailNickname + "@" + organizationDomain,
                PasswordProfile = new Graph.PasswordProfile
                {
                    Password = "Beat@Apple",
                    ForceChangePasswordNextSignIn = false
                }
            };

            var usersRequest = client.Users.Request();
            await usersRequest.AddAsync(user);
            return Content(user.UserPrincipalName);
        }


        public async Task<ActionResult> GetUserToken()
        {
            var tenantID = ClaimsPrincipal.Current.GetTenantId();
            var userObjectId = ClaimsPrincipal.Current.GetObjectIdentifier();
            // Change it to id of tyler.lu@canvizedu.onmicrosoft.com
            userObjectId = "02b8ac5a-aad1-4cb3-a0db-418a4e09a764";

            var authority = string.Format("{0}{1}", Constants.AADInstance, tenantID);
            var context = new AuthenticationContext(authority, new ADALTokenCache(userObjectId));

            var userIdentifier = new UserIdentifier(userObjectId, UserIdentifierType.UniqueId);
            var clientCredential = new ClientCredential(Constants.AADClientId, Constants.AADClientSecret);
            var autoResult = await context.AcquireTokenSilentAsync(Constants.Resources.AADGraph, clientCredential, userIdentifier);
            return Content(autoResult.AccessToken);
        }

        public async Task<ActionResult> GetAppRoleAssignment()
        {
            var client = await AuthenticationHelper.GetActiveDirectoryClientAsync(Permissions.Delegated);

            //// tyler.lu@canvizedu.onmicrosoft.com
            //var userFetcher = client.Users.GetByObjectId("02b8ac5a-aad1-4cb3-a0db-418a4e09a764");

            var resourceId = new Guid("9f89e2d6-aff2-4fc0-a64c-b8a5b59b5f08"); //EDUGraphAPI
            var appRoleAssignment = await client.Me.AppRoleAssignments
                   .Where(i => i.ResourceId == resourceId)
                   .ExecuteFirstOrDefaultAsync();
            return Content(appRoleAssignment?.ResourceDisplayName);
        }

        public async Task<ActionResult> RemoveAndAddAppRoleAssignment()
        {
            var client = await AuthenticationHelper.GetActiveDirectoryClientAsync(Permissions.Delegated);

            var resourceId = new Guid("52ecdf1b-896a-48df-accf-82e894754424");
            var resourceDisplayName = "Graph Explorer";

            // Get the appRoleAssignment and delete it
            var appRoleAssignment = await client.Me.AppRoleAssignments
                .Where(i => i.ResourceId == resourceId)
                .ExecuteFirstOrDefaultAsync();
            if (appRoleAssignment != null)
                await appRoleAssignment.DeleteAsync();

            var me = await client.Me.ExecuteAsync();
            var appRoleAssignment2 = new AppRoleAssignment
            {
                Id = Guid.Empty,
                PrincipalDisplayName = me.DisplayName,
                PrincipalId = new Guid(me.ObjectId),
                PrincipalType = "User",
                ResourceDisplayName = resourceDisplayName,
                ResourceId = resourceId
            };
            await client.Me.AppRoleAssignments.AddAppRoleAssignmentAsync(appRoleAssignment2);

            return Content("Added!");
        }


        public ActionResult ParseDifferentialQueryResult()
        {
            var json = @"{
                    'aad.deltaLink': 'http://canviz.com',
                    'value': [
                        {
                            objectId: 001,
                            displayName: 'Tyler Lu',
                            mail: 'tyler.lu@canviz.com'
                        },
                        {
                            objectId: 002,
                            mail: 'benny.zhang@canviz.com',
                            mobile: '13800000000'
                        }
                    ]
                }";
            var result = DeltaResultParser.Parse<DataSync.User>(json);
            return PlainText(result);
        }

        public async Task<ActionResult> DifferentialQuery()
        {
            var accessToken = await AuthenticationHelper.GetAccessTokenAsync(Constants.Resources.AADGraph, Permissions.Delegated);
            var service = new DifferentialQueryService(() => Task.FromResult(accessToken));

            var url = "https://graph.windows.net/canvizEDU.onmicrosoft.com/users?api-version=1.5&deltaLink=b5_xGr7JDSFXXQCGvZt-hfxvcZRhavmL6x-ad8AuawcARNz6VNITDlcvn5QnrZTc5kGigEFONj7qA-aw8gnW-_2Yk8YRh-NyIVTjtvt2RszyVfS-ZIArJK_KeAaWpQqdw4MVsbMbiwAA1PF3P9qrf82GtgYqdTqdszJTrcL6OsmCdeh4fBusjjVMn2tjTRw1-oc2_FgNtCAVJ2ukfNpjAg3K2NI8iNizJmx3Vw_s--RvcKwLOfmiL2buedH2WCbBfIWUdhp8EOdz0pKEfzpqEre9XvpqzbGfhdisbNkny4qkGk4zbzKn7ZyrN1PchZ7AqcPQXSvdtpTUuG565gONuv57rHUDy0K44q_CIqg2CP3kFJgqgFd0ELkUGVS9LAjEHb9h6aFPveqbF2lDSdpwsrNVAe8cx30Ox5Z94L4NavS0D9q0m-JRY_1m6_wQLSZ5K6ppy7DUJM9n-7fepJu9cRY81BmHz3iUNDVKNbqMKZgdTGriv4StgkiNtWJyCdgi6Ft3xnOifo_o0SwDrk0l0NkgR9CNFPdkxgaSyhaREtL6OiANCXMMEEDHVV2ww_A4pfnuze_gOfLqWiUJEZ0uy0771ml_1yHXErUVg_5TAQrRb0ISBJ4ZjJj2J4zNFdNWnDB433LBlk7TlFTmRF-ieyKgR2ozFj-3uABnzc8vt9kHavqixygrQcB1ip-GtPn8b3mDZirCJhungVNxMDD-h1n1j8EQf0QBFbAgqOnhbwFOTPYB2pJdnbCwgzDr3-1afwABc4gguiSQCxcjKbfHE3bU-cvXVyEJWZY58Nm2pabKGhkdLsqP2ap62yX4OVMHhRv_XdNaoltKGERZ_UUphYVG-s8PHmtgIocgnBDzXczDF9SwNtG-MY5nJOP_q9OhXoBrAmHdATMmT3xwO02XIbwTogVqBjxT8ftNwy8jXf6oqeMipY4G0JbJWgJR_nDeUxuZ_3E-eA6ZwmG1EkcvpETiwok5Ghylnhdph2G_EQ7mxcohH7-gQSI0H1_xDa13fO63nsTOTl6rExuioaJBt_C-v7qYLACFWmjT9oS1HN7-my5dzy_8nBpR0cNSeJ0B6an4r_VDbQAUvkVP-iG_U2iXES2q4jwR-f_014lRAqD-I96Tg1saJgcd5Y7CfYNHGicUFm3tRNYdvtsOT8TCWr-x-jyHSV22q7_KcZ8eGCHNFxIqlys32PwArMUZ4TtUluSN0nsptgtQT8VujPPrhDMMEupMsWvlHLAMjRnXpr7dLuOEv6Zhsq5L1zp_UDFtjMj6Z4G3H4hp10aVmboO-eta2T1n1NkZvRDOvHvQIth_B0yrXk4d9vbeBDwddVEBVxnk2O2luJ_VYHOUMG9ZHw.UC0Ve5rjD4JwTIKr8-kbJyQRkTx3ayqJZ4pL6IpUnBI";
            var result = await service.QueryAsync<DataSync.User>(url);
            return PlainText(result);
        }

        public async Task<ActionResult> SyncUsers()
        {
            var stringBuilder = new StringBuilder();
            var textWriter = new StringWriter(stringBuilder);

            var dbContext = DependencyResolver.Current.GetService<ApplicationDbContext>();
            var accessToken = await AuthenticationHelper.GetAccessTokenAsync(Constants.Resources.AADGraph, Permissions.Delegated);
            var service = new UserSyncService(dbContext, tenantId => Task.FromResult(accessToken), textWriter);
            await service.SyncAsync();

            return Content(stringBuilder.ToString());
        }

        private ContentResult PlainText(DeltaResult<Delta<DataSync.User>> result)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"deltaLink: {result.DeltaLink}");
            stringBuilder.AppendLine($"nextLink: {result.NextLink}");
            stringBuilder.AppendLine();

            foreach (var item in result.Items)
            {
                stringBuilder.AppendFormat(
                    "objectId: {0}, modified properties: {1} \r\n\r\n",
                    item.Entity.ObjectId,
                    String.Join(", ", item.ModifiedPropertyNames));

            }
            return Content(stringBuilder.ToString(), "text/plain");
        }


        public async Task<ActionResult> GroupFiles(string groupId = "beb7fee1-a41a-411c-ab00-e9f3c99a8960")
        {
            var client = await AuthenticationHelper.GetGraphServiceClientAsync(Permissions.Delegated);

            var files = new List<Graph.DriveItem>();

            // TODO: Get Thumbnails
            var result = await client.Groups[groupId].Drive.Root.ItemWithPath("Reading List").Children
                .Request()
                .GetAsync();

            while (true)
            {
                files.AddRange(result.CurrentPage);
                if (result.NextPageRequest == null) break;
                result = await result.NextPageRequest.GetAsync();
            }

            foreach (var file in files)
            {
                Console.WriteLine(file.WebUrl);
                Console.WriteLine(file.Thumbnails);
            }

            return Content($"Get {files.Count} file(s).");
        }

    }
}