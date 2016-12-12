using Microsoft.Education.Data;
using System.Collections.Generic;
using System.Linq;

namespace EDUGraphAPI.Web.ViewModels
{
    public class SectionsViewModel
    {
        public SectionsViewModel(string userEmail, School School, IEnumerable<Section> sections, IEnumerable<Section> mySections)
        {
            this.UserEmail = userEmail;
            this.School = School;
            this.Sections = sections.ToList();
            this.MySections = mySections.ToList();
        }

        public string UserEmail { get; set; }
        
        public School School { get; set; }

        public List<Section> Sections { get; set; }
        public List<Section> MySections { get; set; }
        public bool IsMy(Section section, List<Section> mySections)
        {
            if (MySections == null)
                return false;
            return mySections.Where(c => c.Email == section.Email).Count() > 0 ? true : false;
        }
    }
}