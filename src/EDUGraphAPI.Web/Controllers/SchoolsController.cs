using EDUGraphAPI.Data;
using EDUGraphAPI.Utils;
using EDUGraphAPI.Web.Infrastructure;
using EDUGraphAPI.Web.Models;
using EDUGraphAPI.Web.Services;
using EDUGraphAPI.Web.ViewModels;
<<<<<<< HEAD
using System.Collections.Generic;
=======
>>>>>>> origin/master
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EDUGraphAPI.Web.Controllers
{
    [HandleAdalException, EduAuthorize]
    public class SchoolsController : Controller
    {
        private ApplicationService applicationService;
        private ApplicationDbContext dbContext;
        public SchoolsController(ApplicationService applicationService, ApplicationDbContext dbContext)
        {
            this.applicationService = applicationService;
            this.dbContext = dbContext;
        }

        //
        // GET: /Schools/Index
        public async Task<ActionResult> Index()
        {
            var userContext = await applicationService.GetUserContextAsync();
            if (!userContext.AreAccountsLinked)
            {
<<<<<<< HEAD
                return View(new SchoolsViewModel() { AreAccountsLinked = false,IsLocalAccount = userContext.IsLocalAccount });
=======
                return View(new SchoolsViewModel() { AreAccountsLinked = false });
>>>>>>> origin/master
            }
            var schoolsService = await GetSchoolsServiceAsync();
            var model = await schoolsService.GetSchoolsViewModelAsync(userContext);
            model.AreAccountsLinked = userContext.AreAccountsLinked;
<<<<<<< HEAD
            
=======
>>>>>>> origin/master
            return View(model);
        }

        //
        // GET: /Schools/48D68C86-6EA6-4C25-AA33-223FC9A27959/Sections
        public async Task<ActionResult> Classes(string schoolId)
        {
            var userContext = await applicationService.GetUserContextAsync();
            var schoolsService = await GetSchoolsServiceAsync();
            var model = await schoolsService.GetSectionsViewModelAsync(userContext, schoolId, false);
            return View(model);
        }

        //
        // GET: /Schools/48D68C86-6EA6-4C25-AA33-223FC9A27959/Sections/My
        public async Task<ActionResult> MyClasses(string schoolId)
        {
            var userContext = await applicationService.GetUserContextAsync();
            var schoolsService = await GetSchoolsServiceAsync();
            var model = await schoolsService.GetSectionsViewModelAsync(userContext, schoolId, true);
            ViewBag.IsMySections = true;
            return View("Classes", model);
        }

        //
        // GET: /Schools/48D68C86-6EA6-4C25-AA33-223FC9A27959/Sections/6510F0FC-53B3-4D9B-9742-84C9C8FA2BE4
        public async Task<ActionResult> ClassDetails(string schoolId, string sectionId)
        {
            var userContext = await applicationService.GetUserContextAsync();

            var graphServiceClient = await AuthenticationHelper.GetGraphServiceClientAsync();
            var group = graphServiceClient.Groups[sectionId];

            var schoolsService = await GetSchoolsServiceAsync();
            var model = await schoolsService.GetSectionDetailsViewModelAsync(schoolId, sectionId, group);
            model.IsStudent = userContext.IsStudent;
            model.O365UserId = userContext.User.O365UserId;
<<<<<<< HEAD
            model.MyFavoriteColor = userContext.User.FavoriteColor;
=======
>>>>>>> origin/master
            return View(model);
        }

        private async Task<SchoolsService> GetSchoolsServiceAsync()
        {
            var educationServiceClient = await AuthenticationHelper.GetEducationServiceClientAsync();
            return new SchoolsService(educationServiceClient, dbContext);
<<<<<<< HEAD
        }

        [HttpPost]
        public async Task<JsonResult> SaveEditSeats(List<SaveEditSeatsViewModel> seats)
        {
            await applicationService.SaveEditSeats(seats);
            return Json("");
=======
>>>>>>> origin/master
        }
    }
}