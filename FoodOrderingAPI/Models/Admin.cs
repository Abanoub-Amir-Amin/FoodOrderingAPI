using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class Admin
    {
        [Key, ForeignKey(nameof(User))]
        public int AdminID { get; set; }

        public User User { get; set; }

        public ICollection<ComplaintChat> ComplaintChats { get; set; }
    }
}
