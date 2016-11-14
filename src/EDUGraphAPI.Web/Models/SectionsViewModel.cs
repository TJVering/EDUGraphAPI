using Microsoft.Education.Data;
using System.Collections.Generic;
using System.Linq;

namespace EDUGraphAPI.Web.ViewModels
{
    public class SectionsViewModel
    {
        public SectionsViewModel(string userDisplayName, School School, IEnumerable<Section> sections)
        {
            this.UserDisplayName = userDisplayName;
            this.School = School;
            this.Sections = sections.ToList();
        }

        public string UserDisplayName { get; set; }

        public School School { get; set; }

        public List<Section> Sections { get; set; }
    }
}