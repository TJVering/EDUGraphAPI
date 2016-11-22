﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDUGraphAPI.Web.Models
{
    public class AboutMeViewModel
    {
        public string Username { get; set; }
        public string MyFavoriteColor { get; set; }
        public List<string> FavoriteColors { get; set; }
        public List<string> Groups { get; set; }
    }
}