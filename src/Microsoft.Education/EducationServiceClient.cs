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

        public Task<School> GetSchoolAsync(string objectId)
        {
            return HttpGetObjectAsync<School>($"administrativeUnits/{objectId}?api-version=beta");
        }

        #endregion

        #region sections
        public Task<Section[]> GetAllSectionsAsync(string schoolId)
        {
            var relativeUrl = $"/groups?api-version=beta&$expand=members&$filter=extension_fe2174665583431c953114ff7268b7b3_Education_ObjectType%20eq%20'Section'%20and%20extension_fe2174665583431c953114ff7268b7b3_Education_SyncSource_SchoolId%20eq%20'{schoolId}'";
            return HttpGetArrayAsync<Section>(relativeUrl);
        }

        public async Task<Section[]> GetMySectionsAsync(string schoolId)
        {
            var me = await HttpGetObjectAsync<SectionUser>("/me?api-version=1.5");
            var sections = await GetAllSectionsAsync(schoolId);

            return sections
                .Where(i => i.Members.Any(j => j.Email == me.Email))
                .ToArray();
        }

        public Task<SectionUser[]> GetSectionUsersAsync(string sectionId)
        {
            return HttpGetArrayAsync<SectionUser>($"groups/{sectionId}/members?api-version=beta");
        }

        public async Task<Section> GetSectionAsync(string sectionId)
        {
            return await HttpGetObjectAsync<Section>($"groups/{sectionId}?api-version=beta&$expand=members");
        }

        #endregion

        #region student and teacher
        public Task<Student> GetStudentAsync()
        {
            return HttpGetObjectAsync<Student>("me?api-version=1.5");
        }

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