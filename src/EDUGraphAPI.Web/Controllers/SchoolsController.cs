using EDUGraphAPI.Utils;
using EDUGraphAPI.Web.Infrastructure;
using EDUGraphAPI.Web.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EDUGraphAPI.Web.Controllers
{
    [HandleAdalException, EduAuthorize, LinkedOrO365UsersOnly]
    public class SchoolsController : Controller
    {
        private ApplicationService applicationService;

        public SchoolsController(ApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        //
        // GET: /Schools/Index
        public async Task<ActionResult> Index()
        {
            var userContext = await applicationService.GetUserContextAsync();
            var schoolsService = await GetSchoolsServiceAsync();
            var model = await schoolsService.GetSchoolsViewModelAsync(userContext);
            return View(model);
        }

        //
        // GET: /Schools/48D68C86-6EA6-4C25-AA33-223FC9A27959/Sections
        public async Task<ActionResult> Sections(string schoolId)
        {
            var userContext = await applicationService.GetUserContextAsync();
            var schoolsService = await GetSchoolsServiceAsync();
            var model = await schoolsService.GetSectionsViewModelAsync(userContext, schoolId, false);
            return View(model);
        }

        //
        // GET: /Schools/48D68C86-6EA6-4C25-AA33-223FC9A27959/Sections/My
        public async Task<ActionResult> MySections(string schoolId)
        {
            var userContext = await applicationService.GetUserContextAsync();
            var schoolsService = await GetSchoolsServiceAsync();
            var model = await schoolsService.GetSectionsViewModelAsync(userContext, schoolId, true);
            ViewBag.IsMySections = true;
            return View("Sections", model);
        }

        //
        // GET: /Schools/48D68C86-6EA6-4C25-AA33-223FC9A27959/Sections/6510F0FC-53B3-4D9B-9742-84C9C8FA2BE4
        public async Task<ActionResult> SectionDetails(string schoolId, string sectionId)
        {
            var graphServiceClient = await AuthenticationHelper.GetGraphServiceClientAsync();
            var group = graphServiceClient.Groups[sectionId];

            var schoolsService = await GetSchoolsServiceAsync();
            var model = await schoolsService.GetSectionDetailsViewModelAsync(schoolId, sectionId, group);
            return View(model);
        }

        private async Task<SchoolsService> GetSchoolsServiceAsync()
        {
            var educationServiceClient = await AuthenticationHelper.GetEducationServiceClientAsync();
            return new SchoolsService(educationServiceClient);
        }
    }
}