using Microsoft.IdentityModel.Tokens;
using System;

namespace EDUGraphAPI.Web.Exceptions
{

    [Serializable]
    public class TenantNotByAdminConsentException : SecurityTokenValidationException
    {
        public TenantNotByAdminConsentException() { }
        public TenantNotByAdminConsentException(string message) : base(message) { }
        public TenantNotByAdminConsentException(string message, Exception inner) : base(message, inner) { }
    }
}