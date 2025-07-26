using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FoodOrderingAPI
{
    public class ApplicationDBContext: IdentityDbContext<User, IdentityRole<string>, string>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options): base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();


            // Admin <-> User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Admin)
                .WithOne(a => a.User)
                .HasForeignKey<Admin>(a => a.UserId);

            // Customer <-> User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Customer)
                .WithOne(c => c.User)
                .HasForeignKey<Customer>(c => c.UserID);

            // Restaurant <-> User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Restaurant)
                .WithOne(r => r.User)
                .HasForeignKey<Restaurant>(r => r.UserId);

            // DeliveryMan <-> User
            modelBuilder.Entity<User>()
                .HasOne(u => u.DeliveryMan)
                .WithOne(d => d.User)
                .HasForeignKey<DeliveryMan>(d => d.UserId);

            modelBuilder.Entity<ComplaintChat>()
                .HasOne(cc => cc.Admin)
                .WithMany(a => a.ComplaintChats)
                .HasForeignKey(cc => cc.AdminID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ComplaintChat>()
                .HasOne(cc => cc.Customer)
                .WithMany(c => c.ComplaintChats)
                .HasForeignKey(cc => cc.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);  

            

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Restaurant)
                .WithMany(r => r.Orders)
                .HasForeignKey(o => o.RestaurantID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Discount>()
                .HasOne(d => d.Restaurant)
                .WithMany(r => r.Discounts)
                .HasForeignKey(d => d.RestaurantID)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Discount>()
                .HasOne(d => d.Item)
                .WithMany(i => i.Discounts)
                .HasForeignKey(d => d.ItemID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShoppingCartItem>()
                .HasOne(sci => sci.ShoppingCart)
                .WithMany(sc => sc.ShoppingCartItems)
                .HasForeignKey(sci => sci.CartID)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<ShoppingCartItem>()
                .HasOne(sci => sci.Item)
                .WithMany(i => i.ShoppingCartItems)
                .HasForeignKey(sci => sci.ItemID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                 .HasOne(r => r.Customer)
                 .WithMany(c => c.Reviews)
                 .HasForeignKey(r => r.CustomerID)
                 .OnDelete(DeleteBehavior.Restrict);  

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Order)
                .WithMany(o => o.Reviews)
                .HasForeignKey(r => r.OrderID)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Restaurant)
                .WithMany(rest => rest.Reviews)
                .HasForeignKey(r => r.RestaurantID)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.Order)
                .WithMany(oi => oi.OrderItems)
                .HasForeignKey(oi => oi.OrderID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User_ConnectionId>()
               .HasKey(uc => new { uc.UserId, uc.ConnectionId });
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<DeliveryMan> DeliveryMen { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ComplaintChat> ComplaintChats { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<RewardHistory> RewardHistories { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<User_ConnectionId> User_ConnectionId { get; set; }

    }
}
