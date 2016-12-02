using System.ComponentModel.DataAnnotations;

namespace EDUGraphAPI.Data
{
   public class ClassroomSeatingArrangements
    {
        [Key]
<<<<<<< HEAD
        public int Id { get; set; }
        public string O365UserId { get; set; }
        public int Position { get; set; }

        public string ClassId { get; set; }
=======
        public string O365UserId { get; set; }
        public int Position { get; set; }
>>>>>>> origin/master
    }
}
