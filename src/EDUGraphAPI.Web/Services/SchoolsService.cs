using EDUGraphAPI.Data;
using EDUGraphAPI.Web.Models;
using EDUGraphAPI.Web.ViewModels;
using Microsoft.Education;
using Microsoft.Education.Data;
using Microsoft.Graph;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EDUGraphAPI.Web.Services
{
    /// <summary>
    /// A service class used to get education data by controllers
    /// </summary>
    public class SchoolsService
    {
        private EducationServiceClient educationServiceClient;
        private ApplicationDbContext dbContext;

        public SchoolsService(EducationServiceClient educationServiceClient,ApplicationDbContext dbContext)
        {
            this.educationServiceClient = educationServiceClient;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Get SchoolsViewModel
        /// </summary>
        public async Task<SchoolsViewModel> GetSchoolsViewModelAsync(UserContext userContext)
        {
            var currentUser = userContext.IsStudent
                ? await educationServiceClient.GetStudentAsync() as SectionUser
                : await educationServiceClient.GetTeacherAsync() as SectionUser;

            var schools = (await educationServiceClient.GetSchoolsAsync())
                .OrderBy(i => i.Name)
                .ToArray();
            BingMapService mapServices = new BingMapService();
            for (var i = 0; i < schools.Count(); i++)
            {
                var address = string.Format("{0}/{1}/{2}", schools[i].State, HttpUtility.HtmlEncode(schools[i].City), HttpUtility.HtmlEncode(schools[i].Address));
                if (!string.IsNullOrEmpty(schools[i].Address))
                {
                    var longitudeAndLatitude = await mapServices.GetLongitudeAndLatitudeByAddress(address);
                    if (longitudeAndLatitude.Count() == 2)
                    {
                        schools[i].Latitude = longitudeAndLatitude[0].ToString();
                        schools[i].Longitude = longitudeAndLatitude[1].ToString();
                    }
                }
                else
                {
                    if(string.IsNullOrEmpty(schools[i].Zip))
                        schools[i].Address = "-";
                }
            }

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

        /// <summary>
        /// Get SectionsViewModel of the specified school
        /// </summary>
        public async Task<SectionsViewModel> GetSectionsViewModelAsync(UserContext userContext, string objectId, bool mySections)
        {
            var school = await educationServiceClient.GetSchoolAsync(objectId);
            var sections = mySections
                ? await educationServiceClient.GetMySectionsAsync(school.SchoolId)
                : await educationServiceClient.GetAllSectionsAsync(school.SchoolId);

            return new SectionsViewModel(userContext.UserO365Email, school, sections.OrderBy(c => c.CombinedCourseNumber));
        }

        /// <summary>
        /// Get teachers and students of the specified school
        /// </summary>
        public async Task<SchoolUsersViewModel> GetSchoolUsersAsync(string objectId)
        {
            var school = await educationServiceClient.GetSchoolAsync(objectId);
            var users = await educationServiceClient.GetMembersAsync(objectId);
            return new SchoolUsersViewModel(school,users);
        }

        /// <summary>
        /// Get SectionDetailsViewModel of the specified section
        /// </summary>
        public async Task<SectionDetailsViewModel> GetSectionDetailsViewModelAsync(string schoolId, string classId, IGroupRequestBuilder group)
        {
            var school = await educationServiceClient.GetSchoolAsync(schoolId);
            var section = await educationServiceClient.GetSectionAsync(classId);
            var driveRootFolder = await group.Drive.Root.Request().GetAsync();
            foreach (var user in section.Students)
            {
                var seat= dbContext.ClassroomSeatingArrangements.Where(c => c.O365UserId == user.O365UserId && c.ClassId==classId).FirstOrDefault();
                user.Position = (seat == null ? 0 : seat.Position);
            }
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
        
        /// <summary>
        /// Get my classes
        /// </summary>
        public async Task<List<string>> GetMyClassesAsync(UserContext userContext)
        {
            List<string> results = new List<string>();
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
            if(myFirstSchool !=null)
            {
                var myClasses = (await educationServiceClient.GetMySectionsAsync(myFirstSchool.SchoolId));
                foreach (var item in myClasses)
                {
                    results.Add(item.DisplayName);
                }
            }
            return results;
        }
    }
}