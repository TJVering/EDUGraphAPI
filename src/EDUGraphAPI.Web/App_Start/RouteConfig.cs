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
                name: "Sections",
                url: "Schools/{schoolId}/Sections",
                defaults: new { controller = "Schools", action = "Sections" }
            );

            routes.MapRoute(
                name: "MySections",
                url: "Schools/{schoolId}/Sections/My",
                defaults: new { controller = "Schools", action = "MySections" }
            );

            routes.MapRoute(
                name: "SectionDetails",
                url: "Schools/{schoolId}/Sections/{sectionId}",
                defaults: new { controller = "Schools", action = "SectionDetails" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
