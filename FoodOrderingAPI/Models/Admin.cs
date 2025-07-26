using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class Admin 
    {
        [Key, ForeignKey(nameof(User))]
        public string AdminID { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public ICollection<ComplaintChat> ComplaintChats { get; set; } = new List<ComplaintChat>();
    }
}
