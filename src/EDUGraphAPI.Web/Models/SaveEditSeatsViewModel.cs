using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDUGraphAPI.Web.Models
{
    public class SaveEditSeatsViewModel
    {
        public string O365UserId { get; set; }
        public int Position { get; set; }

        public string ClassId { get; set; }
    }
}