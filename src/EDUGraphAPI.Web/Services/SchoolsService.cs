using EDUGraphAPI.Web.Models;
using EDUGraphAPI.Web.ViewModels;
using Microsoft.Education;
using Microsoft.Education.Data;
using Microsoft.Graph;
using System.Linq;
using System.Threading.Tasks;

namespace EDUGraphAPI.Web.Services
{
    public class SchoolsService
    {
        private EducationServiceClient educationServiceClient;

        public SchoolsService(EducationServiceClient educationServiceClient)
        {
            this.educationServiceClient = educationServiceClient;
        }

        public async Task<SchoolsViewModel> GetSchoolsViewModelAsync(UserContext userContext)
        {
            var currentUser = userContext.IsStudent
                ? await educationServiceClient.GetStudentAsync() as SectionUser
                : await educationServiceClient.GetTeacherAsync() as SectionUser;

            var schools = (await educationServiceClient.GetSchoolsAsync())
                .OrderBy(i => i.Name)
                .ToArray();
            var mySchools = schools
                .Where(i => i.SchoolId == currentUser.SchoolId)
                .ToArray();

            var myFirstSchool = mySchools.FirstOrDefault();
            var grade = userContext.IsStudent ? currentUser.EducationGrade : myFirstSchool?.EducationGrade;

            var sortedSchools = mySchools
                .Union(schools.Except(mySchools));
            return new SchoolsViewModel(sortedSchools)
            {
                IsStudent = userContext.IsStudent,
                UserId = currentUser.UserId,
                EducationGrade = grade,
                UserDisplayName = currentUser.DisplayName,
                MySchoolId = currentUser.SchoolId,
                BingMapKey = Constants.BingMapKey
            };
        }

        public async Task<SectionsViewModel> GetSectionsViewModelAsync(UserContext userContext, string objectId, bool mySections)
        {
            var school = await educationServiceClient.GetSchoolAsync(objectId);
            var sections = mySections
                ? await educationServiceClient.GetMySectionsAsync(school.SchoolId)
                : await educationServiceClient.GetAllSectionsAsync(school.SchoolId);

            return new SectionsViewModel(userContext.UserO365Email, school, sections.OrderBy(c => c.CombinedCourseNumber));
        }

        public async Task<SectionDetailsViewModel> GetSectionDetailsViewModelAsync(string schoolId, string sectionId, IGroupRequestBuilder group)
        {
            var school = await educationServiceClient.GetSchoolAsync(schoolId);
            var section = await educationServiceClient.GetSectionAsync(sectionId);
            var driveRootFolder = await group.Drive.Root.Request().GetAsync();

            return new SectionDetailsViewModel
            {
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