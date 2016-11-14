using EDUGraphAPI.Web.Infrastructure;
using EDUGraphAPI.Web.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EDUGraphAPI.Web.Controllers
{
    [EduAuthorize]
    public class HomeController : Controller
    {
        private ApplicationService applicationService;
        
        public HomeController(ApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        public async Task<ActionResult> Index()
        {
            var context = await applicationService.GetUserContext();
            if (context.IsLinked) return RedirectToAction("Index", "Schools");
            return View(context);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}