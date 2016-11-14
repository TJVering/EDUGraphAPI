using EDUGraphAPI.Web.Models;
using Microsoft.Graph;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDUGraphAPI.Web.Services.GraphClients
{
    public class MSGraphClient : IGraphClient
    {
        private GraphServiceClient graphServiceClient;

        public MSGraphClient(GraphServiceClient graphServiceClient)
        {
            this.graphServiceClient = graphServiceClient;
        }

        public async Task<UserInfo> GetCurrentUserAsync()
        {
            var me = await graphServiceClient.Me.Request()
                .Select("id,givenName,surname,userPrincipalName,assignedLicenses")
                .GetAsync();
            return new UserInfo
            {
                Id = me.Id,
                GivenName = me.GivenName,
                Surname = me.Surname,
                UserPrincipalName = me.UserPrincipalName,
                Roles = GetRoles(me).ToArray()
            };
        }

        public async Task<TenantInfo> GetTenantAsync(string tenantId)
        {
            var tenants = await graphServiceClient.Organization.Request()
                .Filter($"id eq '{tenantId}'")
                .Top(1)
                .GetAsync();
            var tenant = tenants.CurrentPage.FirstOrDefault();
            return new TenantInfo
            {
                Id = tenant.Id,
                Name = tenant.DisplayName
            };
        }

        public IEnumerable<string> GetRoles(User user)
        {
            if (user.GivenName.ToLower().Contains("admin")) // TODO: Check if current user is admin
                yield return Constants.Roles.Admin;
            if (user.AssignedLicenses.Any(i => i.SkuId == Constants.O365ProductLicenses.Faculty))
                yield return Constants.Roles.Faculty;
            if (user.AssignedLicenses.Any(i => i.SkuId == Constants.O365ProductLicenses.Student))
                yield return Constants.Roles.Student;
        }
    }
}