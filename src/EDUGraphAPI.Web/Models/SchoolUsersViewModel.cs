using Microsoft.Education.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDUGraphAPI.Web.Models
{
    public class SchoolUsersViewModel
    {
        public SchoolUsersViewModel( School School, SectionUser[] users)
        {
            this.Users = users;
            this.School = School;
        }

        public School School { get; set; }
        public SectionUser[] Users { get; set; }
    }
}