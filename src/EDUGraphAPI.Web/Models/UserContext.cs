using EDUGraphAPI.Data;
using Microsoft.AspNet.Identity;
using System.Web;

namespace EDUGraphAPI.Web.Models
{
    public class UserContext
    {
        public UserContext(HttpContext httpContext, ApplicationUser user)
        {
            this.HttpContext = httpContext;
            this.User = user;
        }

        public HttpContext HttpContext { get; private set; }

        public ApplicationUser User { get; private set; }

        public string UserDisplayName => HttpContext.User.Identity.GetUserName();

        public bool IsLocalAccount => User != null && User.Id == HttpContext.User.Identity.GetUserId();

        public bool IsO365Account => !IsLocalAccount;

        public bool IsLinked => User != null && User.O365UserId.IsNotNullAndEmpty();

        public bool IsFaculty => HttpContext.User.IsInRole(Constants.Roles.Faculty);

        public bool IsStudent => HttpContext.User.IsInRole(Constants.Roles.Student);

        public bool IsAdmin => HttpContext.User.IsInRole(Constants.Roles.Admin);

        public bool IsTenantConsented => User != null && User.Organization != null && User.Organization.IsAdminConsented;

        public string UserO365Email => IsLinked
            ? User.O365Email
            : (IsO365Account ? HttpContext.User.Identity.Name : null);
    }
}