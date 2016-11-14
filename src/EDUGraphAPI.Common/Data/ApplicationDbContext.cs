using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace EDUGraphAPI.Data
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Organization> Organizations { get; set; }

        public DbSet<UserTokenCache> UserTokenCacheList { get; set; }

        public DbSet<DataSyncRecord> DataSyncRecords { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }


    //public class ApplicationDbContext : DbContext
    //{
    //    public ApplicationDbContext()
    //        : base("DefaultConnection")
    //    {
    //    }

    //    public DbSet<User> Users { get; set; }

    //    public DbSet<Tenant> Tenants { get; set; }

    //    public DbSet<UserTokenCache> UserTokenCacheList { get; set; }
    //}

    //public class EDUGraphAPIDBInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    //{

    //    protected override void Seed(ApplicationDbContext context)
    //    {
    //        //Teacher
    //        context.Users.Add(new ApplicationUser { FirstName = "Bill", LastName = "Sluss", Email = "Billsluss@canvizEDU.onmicrosoft.com", Password = "pass@word", Token = "", O365Email = "", UserType = UserType.Teacher });

    //        //Student
    //        context.Users.Add(new ApplicationUser { FirstName = "Bill", LastName = "Brown", Email = "Bbrown@canvizEDU.onmicrosoft.com", Password = "pass@word", Token = "", O365Email = "", UserType = UserType.Student });
    //        context.SaveChanges();
    //        base.Seed(context);
    //    }
    //}
}