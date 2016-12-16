using Microsoft.Education.Data;
using System.Collections.Generic;
using System.Linq;

namespace EDUGraphAPI.Web.ViewModels
{
    public class SectionsViewModel
    {
        public SectionsViewModel(string userEmail, School School, IEnumerable<Section> sections, IEnumerable<Section> mySections, string nextLinkOfSections)
        {
            this.UserEmail = userEmail;
            this.School = School;
            this.Sections = sections.ToList();
            this.MySections = mySections.ToList();
            this.NextLinkOfSections = nextLinkOfSections;
        }

        public string UserEmail { get; set; }
        public School School { get; set; }
        public List<Section> Sections { get; set; }
        public string NextLinkOfSections { get; set; }
        public List<Section> MySections { get; set; }

        public bool IsMy(Section section)
        {
            return MySections != null && MySections.Any(c => c.Email == section.Email);
        }
    }
}