using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static EDUGraphAPI.Constants;

namespace EDUGraphAPI.Web.Models
{
    public class AboutMeViewModel
    {
        public string Username { get; set; }
        public string MyFavoriteColor { get; set; }
        public List<ColorEntity> FavoriteColors { get; set; }
        public List<string> Groups { get; set; }

        public bool ShowFavoriteColor { get; set; }
    }
}