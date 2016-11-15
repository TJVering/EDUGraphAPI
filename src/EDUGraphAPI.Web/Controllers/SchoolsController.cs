using EDUGraphAPI.Web.Infrastructure;
using EDUGraphAPI.Web.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EDUGraphAPI.Web.Controllers
{
    [HandleAdalException, EduAuthorize]
    public class SchoolsController : Controller
    {
        private SchoolsService schoolsService;

        public SchoolsController(SchoolsService schoolsService)
        {
            this.schoolsService = schoolsService;
        }

        public async Task<ActionResult> Index()
        {
            var model = await schoolsService.GetSchoolsViewModelAsync();
            return View(model);
        }

        public async Task<ActionResult> Sections(string schoolId)
        {
            var model = await schoolsService.GetSectionsViewModelAsync(schoolId, false);
            return View(model);
        }

        public async Task<ActionResult> MySections(string schoolId)
        {
            var model = await schoolsService.GetSectionsViewModelAsync(schoolId, true);
            ViewBag.IsMySections = true;
            return View("Sections", model);
        }

        public async Task<ActionResult> SectionDetails(string schoolId, string sectionId)
        {
            var model = await schoolsService.GetSectionDetailsViewModelAsync(schoolId, sectionId);
            return View(model);
        }
    }
}