﻿using System.ComponentModel.DataAnnotations;

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
    }
}