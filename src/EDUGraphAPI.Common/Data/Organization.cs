using System;

namespace EDUGraphAPI.Data
{
    public class Organization
    {
        public int Id { get; set; }

        public string TenantId { get; set; }

        public string Name { get; set; }

        public DateTime Created { get; set; }

        public bool IsAdminConsented { get; set; }

        public string Issuer => $"https://sts.windows.net/{TenantId}/";
    }
}