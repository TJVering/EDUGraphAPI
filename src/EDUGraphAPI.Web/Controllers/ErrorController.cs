using System.Web.Mvc;

namespace EDUGraphAPI.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index(string message)
        {
            return View((object)message);
        }
    }
}