using Microsoft.Education.Data;
using System.Collections.Generic;
using System.Linq;

namespace EDUGraphAPI.Web.ViewModels
{
    public class SectionsViewModel
    {
        public SectionsViewModel(string userEmail, School School, IEnumerable<Section> sections)
        {
            this.UserEmail = userEmail;
            this.School = School;
            this.Sections = sections.ToList();
        }

        public string UserEmail { get; set; }
        
        public School School { get; set; }

        public List<Section> Sections { get; set; }

        public bool IsMy(Section section)
        {
            return section.Members.Any(i => i.Email == UserEmail);
        }
    }
}