using System.ComponentModel.DataAnnotations;

namespace EDUGraphAPI.Data
{
   public class ClassroomSeatingArrangements
    {
        [Key]
        public string O365UserId { get; set; }
        public int Position { get; set; }
    }
}
