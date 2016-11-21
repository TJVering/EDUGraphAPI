using Microsoft.Education.Data;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.Education
{
    public class EducationServiceClient
    {
        private string serviceRoot;
        private Func<Task<string>> accessTokenGetter;

        public EducationServiceClient(Uri serviceRoot, Func<Task<string>> accessTokenGetter)
        {
            this.serviceRoot = serviceRoot.ToString().TrimEnd('/');
            this.accessTokenGetter = accessTokenGetter;
        }

        #region schools
        /// <summary>
        /// Get all schools that exist in the Azure Active Directory tenant. 
        /// Reference URL: https://msdn.microsoft.com/office/office365/api/school-rest-operations#get-all-schools
        /// </summary>
        /// <returns></returns>
        public async Task<School[]> GetSchoolsAsync()
        {
            var schools = await HttpGetArrayAsync<School>("administrativeUnits?api-version=beta");
            BingMapService mapServices = new BingMapService();
            for (var i = 0; i < schools.Count(); i++)
            {
                var address = string.Format("{0}/{1}/{2}", schools[i].State, HttpUtility.HtmlEncode(schools[i].City), HttpUtility.HtmlEncode(schools[i].Address));
                var longitudeAndLatitude = await mapServices.GetLongitudeAndLatitudeByAddress(address);
                if (longitudeAndLatitude.Count() == 2)
                {
                    schools[i].Latitude = longitudeAndLatitude[0].ToString();
                    schools[i].Longitude = longitudeAndLatitude[1].ToString();
                }
            }
            return schools;
        }

        /// <summary>
        /// Get a school by using the object_id.
        /// Reference URL: https://msdn.microsoft.com/office/office365/api/school-rest-operations#get-a-school.
        /// </summary>
        /// <param name="objectId">The Object ID of the school administrative unit in Azure Active Directory.</param>
        /// <returns></returns>
        public Task<School> GetSchoolAsync(string objectId)
        {
            return HttpGetObjectAsync<School>($"administrativeUnits/{objectId}?api-version=beta");
        }

        #endregion

        #region sections
        /// <summary>
        /// Get sections within a school.
        /// Reference URL: https://msdn.microsoft.com/office/office365/api/school-rest-operations#get-sections-within-a-school.
        /// </summary>
        /// <param name="schoolId">The ID of the school in the School Information System (SIS).</param>
        /// <returns></returns>
        public Task<Section[]> GetAllSectionsAsync(string schoolId)
        {
            var relativeUrl = $"/groups?api-version=beta&$expand=members&$filter=extension_fe2174665583431c953114ff7268b7b3_Education_ObjectType%20eq%20'Section'%20and%20extension_fe2174665583431c953114ff7268b7b3_Education_SyncSource_SchoolId%20eq%20'{schoolId}'";
            return HttpGetArrayAsync<Section>(relativeUrl);
        }

        public async Task<Section[]> GetMySectionsAsync(string schoolId)
        {
            //You can get the membership of the student using the query below.
            //Reference URL: https://msdn.microsoft.com/office/office365/api/student-rest-operations#get-section-of-a-student.
            var me = await HttpGetObjectAsync<SectionUser>("/me?api-version=1.5");

            var sections = await GetAllSectionsAsync(schoolId);

            return sections
                .Where(i => i.Members.Any(j => j.Email == me.Email))
                .ToArray();
        }



        /// <summary>
        /// Get a section by using the object_id.
        /// Reference URL: https://msdn.microsoft.com/office/office365/api/section-rest-operations#get-a-section.
        /// </summary>
        /// <param name="sectionId">The Object ID of the section group in Azure Active Directory.</param>
        /// <returns></returns>
        public async Task<Section> GetSectionAsync(string sectionId)
        {
            return await HttpGetObjectAsync<Section>($"groups/{sectionId}?api-version=beta&$expand=members");
        }

        #endregion

        #region student and teacher
        /// <summary>
        /// You can get the current logged in user and check if that user is a student.
        /// Reference URL: https://msdn.microsoft.com/office/office365/api/student-rest-operations#get-current-user.
        /// </summary>
        /// <returns></returns>
        public Task<Student> GetStudentAsync()
        {
            return HttpGetObjectAsync<Student>("me?api-version=1.5");
        }

        /// <summary>
        /// You can get the current logged in user and check if that user is a student.
        /// Reference URL: https://msdn.microsoft.com/office/office365/api/student-rest-operations#get-current-user.
        /// </summary>
        /// <returns></returns>
        public Task<Teacher> GetTeacherAsync()
        {
            return HttpGetObjectAsync<Teacher>("me?api-version=1.5");
        }
        #endregion

        #region HttpGet
        private async Task<string> HttpGetAsync(string relativeUrl)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", await accessTokenGetter());

            var uri = serviceRoot + "/" + relativeUrl;
            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<T> HttpGetObjectAsync<T>(string relativeUrl)
        {
            var responseString = await HttpGetAsync(relativeUrl);
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        private async Task<T[]> HttpGetArrayAsync<T>(string relativeUrl)
        {
            var responseString = await HttpGetAsync(relativeUrl);
            var array = JsonConvert.DeserializeObject<ArrayResult<T>>(responseString);
            return array.Value;
        }
        #endregion
    }
}