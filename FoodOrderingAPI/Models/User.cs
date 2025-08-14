using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FoodOrderingAPI.Models
{
    public class User : IdentityUser
    {
        /*inherited from IdentityUser<string>*/
        // Remove UserId - use base.Id
        // Remove UserName, Email, Password 

        [EnumDataType(typeof(RoleEnum))]
        public RoleEnum Role { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Customer Customer { get; set; }
        public Admin Admin { get; set; }
        public Restaurant Restaurant { get; set; }
        public DeliveryMan DeliveryMan { get; set; }
        public IEnumerable<Notification> Notifications { get; set; } = new HashSet<Notification>();
        public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    }
}
