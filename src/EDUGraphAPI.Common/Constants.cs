using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace EDUGraphAPI
{
    public static class Constants
    {
        public static readonly string AADClientId = ConfigurationManager.AppSettings["ida:ClientId"];
        public static readonly string AADClientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];

        public static readonly string AADInstance = "https://login.microsoftonline.com/";
        public static readonly string Authority = AADInstance + "common/";

        public const string GraphResourceRootUrl = "https://graph.windows.net";

        public static readonly string BingMapKey = ConfigurationManager.AppSettings["BingMapKey"];

        public static readonly string O365GroupConversationsUrl = "https://outlook.office.com/owa/?path=/group/{0}";

        public static readonly string AADCompanyAdminRoleName = "Company Administrator";

        public static readonly List<string> FavoriteColors = ConfigurationManager.AppSettings["FavoriteColors"].ToString().Split(',').ToList();
        public static class Resources
        {
            public static readonly string AADGraph = "https://graph.windows.net";
            public static readonly string MSGraph = "https://graph.microsoft.com";
        }

        public static class Roles
        {
            public static readonly string Admin = "Admin";
            public static readonly string Faculty = "Faculty";
            public static readonly string Student = "Student";
        }

        public static class O365ProductLicenses
        {
            /// <summary>
            /// Microsoft Classroom Preview
            /// </summary>
            public static readonly Guid Classroom = new Guid("80f12768-d8d9-4e93-99a8-fa2464374d34");
            /// <summary>
            /// Office 365 Education for faculty
            /// </summary>
            public static readonly Guid Faculty = new Guid("94763226-9b3c-4e75-a931-5c89701abe66");
            /// <summary>
            /// Office 365 Education for students
            /// </summary>
            public static readonly Guid Student = new Guid("314c4481-f395-4525-be8b-2ec4bb1e9d91");
        }

    }
}