using EDUGraphAPI.Utils;
using EDUGraphAPI.Web.ViewModels;
using Microsoft.Education.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EDUGraphAPI.Web.Services
{
    public class SchoolsService
    {
        private ApplicationService applicationService;

        public SchoolsService(ApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        public async Task<SchoolsViewModel> GetSchoolsViewModelAsync()
        {
            var eduServiceClient = await AuthenticationHelper.GetEducationServiceClientAsync(Permissions.Delegated);
            var userContext = await applicationService.GetUserContext();
            var currentUser = userContext.IsStudent
                ? await eduServiceClient.GetStudentAsync() as SectionUser
                : await eduServiceClient.GetTeacherAsync() as SectionUser;

            var eduServiceClient2 = await AuthenticationHelper.GetEducationServiceClientAsync(Permissions.Application);
            var schools = await eduServiceClient2.GetSchoolsAsync();

            var mySchool = schools.Where(i => i.SchoolId == currentUser.SchoolId).FirstOrDefault();
            var grade = userContext.IsStudent ? currentUser.EducationGrade : mySchool?.EducationGrade;

            return new SchoolsViewModel(schools.OrderByDescending(i => i.Name))
            {
                IsStudent = userContext.IsStudent,
                UserId = currentUser.UserId,
                EducationGrade = grade,
                UserDisplayName = currentUser.DisplayName,
                MySchoolId = currentUser.SchoolId,
                BingMapKey = Constants.BingMapKey
            };
        }

        public async Task<SectionsViewModel> GetSectionsViewModelAsync(string objectId, bool mySections)
        {
            var eduServiceClient = await AuthenticationHelper.GetEducationServiceClientAsync(Permissions.Delegated);

            var userContext = await applicationService.GetUserContext();
            var school = await eduServiceClient.GetSchoolAsync(objectId);
            var sections = mySections
                ? await eduServiceClient.GetMySectionsAsync(school.SchoolId)
                : await eduServiceClient.GetAllSectionsAsync(school.SchoolId);

            return new SectionsViewModel(userContext.UserDisplayName, school, sections.OrderByDescending(c => c.CourseName));
        }

        public async Task<SectionDetailsViewModel> GetSectionDetailsViewModelAsync(string schoolId, string sectionId)
        {
            var eduServiceClient = await AuthenticationHelper.GetEducationServiceClientAsync();
            var userContext = await applicationService.GetUserContext();
            var school = await eduServiceClient.GetSchoolAsync(schoolId);
            var section = await eduServiceClient.GetSectionAsync(sectionId);

            var graphServiceClient = await AuthenticationHelper.GetGraphServiceClientAsync(Permissions.Delegated);
            var group = graphServiceClient.Groups[sectionId];
            var driveRootFolder = await group.Drive.Root.Request().GetAsync();
            
            return new SectionDetailsViewModel
            {
                UserDisplayName = userContext.UserDisplayName,
                School = school,
                Section = section,
                Conversations = await group.Conversations.Request().GetAllAsync(),
                SeeMoreConversationsUrl = string.Format(Constants.O365GroupConversationsUrl, section.Email),
                DriveItems = await group.Drive.Root.Children.Request().GetAllAsync(),
                SeeMoreFilesUrl = driveRootFolder.WebUrl
            };
        }

    }
}