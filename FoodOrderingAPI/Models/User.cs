using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderingAPI.Models
{
    public class User:IdentityUser
    {
        
        [Required]
        [EnumDataType(typeof(RoleEnum))]
        public RoleEnum Role { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

        //public Guid? GroupId { get; set; }
        public IEnumerable<Notification> Notifications { get; set; } = new HashSet<Notification>();

        //public virtual Group? Group { get; set; }
    }
}
