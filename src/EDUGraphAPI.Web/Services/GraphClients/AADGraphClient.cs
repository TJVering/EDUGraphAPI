using EDUGraphAPI.Web.Models;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDUGraphAPI.Web.Services.GraphClients
{
    public class AADGraphClient : IGraphClient
    {
        private ActiveDirectoryClient activeDirectoryClient;

        public AADGraphClient(ActiveDirectoryClient activeDirectoryClient)
        {
            this.activeDirectoryClient = activeDirectoryClient;
        }

        public async Task<UserInfo> GetCurrentUserAsync()
        {
            var me = await activeDirectoryClient.Me.ExecuteAsync();

            return new UserInfo
            {
                Id = me.ObjectId,
                GivenName = me.GivenName,
                Surname = me.Surname,
                UserPrincipalName = me.UserPrincipalName,
                Roles = GetRoles(me).ToArray()
            };
        }

        public async Task<TenantInfo> GetTenantAsync(string tenantId)
        {
            var tenant = await activeDirectoryClient.TenantDetails
                .Where(i => i.ObjectId == tenantId)
                .ExecuteSingleAsync();

            return new TenantInfo
            {
                Id = tenant.ObjectId,
                Name = tenant.DisplayName
            };
        }

        private IEnumerable<string> GetRoles(IUser user)
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