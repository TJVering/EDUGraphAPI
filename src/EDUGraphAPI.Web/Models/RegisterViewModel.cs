﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static EDUGraphAPI.Constants;

namespace EDUGraphAPI.Web.Models
{
    public class EducationRegisterViewModel : RegisterViewModel
    {
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Favorite color")]
        public string FavoriteColor { get; set; }
        public List<ColorEntity> FavoriteColors { get; set; }
    }
}