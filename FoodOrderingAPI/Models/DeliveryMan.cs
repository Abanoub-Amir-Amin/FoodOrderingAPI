﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public enum AccountStatusEnum
    {
        Rejected,
        Active,
        Pending
    }
    public class DeliveryMan
    {
        [Key, ForeignKey(nameof(User))]
        public string DeliveryManID { get; set; }
        public string UserId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; } 
       
        public float? Rank { get; set; } = 0;

        public bool AvailabilityStatus { get; set; } = true;

        [EnumDataType(typeof(AccountStatusEnum))]
        public AccountStatusEnum AccountStatus { get; set; } = AccountStatusEnum.Pending;

        public DateTime? LastOrderDate { get; set; }

        public  User User { get; set; }

        public  ICollection<Order> Orders { get; set; } = new List<Order>();
        
    }

}
