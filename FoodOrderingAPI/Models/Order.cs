﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class Order
    {
        [Key]
        public Guid OrderID { get; set; }

        [ForeignKey(nameof(Customer))]
        public string CustomerID { get; set; }

        [ForeignKey(nameof(Address))]
        public Guid AddressID { get; set; }

        [ForeignKey(nameof(Restaurant))]
        public string RestaurantID { get; set; }

        [ForeignKey(nameof(DeliveryMan))]
        public string? DeliveryManID { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public DateTime? DeliveredAt { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [ForeignKey(nameof(PromoCode))]
        public Guid? PromoCodeID { get; set; }

        public Customer Customer { get; set; }
        public Address Address { get; set; }
        public Restaurant Restaurant { get; set; }
        public DeliveryMan DeliveryMan { get; set; }
        public PromoCode PromoCode { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    }

}
