using System.Web.Mvc;
using System.Web.Routing;

namespace EDUGraphAPI.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute(
                name: "Classes",
                url: "Schools/{schoolId}/Classes",
                defaults: new { controller = "Schools", action = "Classes" }
            );

            routes.MapRoute(
                name: "Users",
                url: "Schools/{schoolId}/Users",
                defaults: new { controller = "Schools", action = "Users" }
            );

            routes.MapRoute(
                name: "MyClasses",
                url: "Schools/{schoolId}/Classes/My",
                defaults: new { controller = "Schools", action = "MyClasses" }
            );

            routes.MapRoute(
                name: "ClassDetails",
                url: "Schools/{schoolId}/Classes/{sectionId}",
                defaults: new { controller = "Schools", action = "ClassDetails" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
