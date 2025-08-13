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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSeeding((context, _) =>
            {
                var admin = context.Set<User>().FirstOrDefault(u => u.Id == "Admin0");
                if (admin == null)
                {
                    admin = new User
                    {
                        Id = "Admin0",
                        UserName = "Admin",
                        PasswordHash = new PasswordHasher<User>().HashPassword(null, "AS_AS_s1"),
                        EmailConfirmed = true,
                        Role = RoleEnum.Admin,
                        CreatedAt = DateTime.UtcNow,
                        LockoutEnabled = false,
                        AccessFailedCount = 0,
                        PhoneNumberConfirmed = true,
                    };
                    context.Set<User>().Add(admin);
                    context.SaveChanges();
                }
            })
            .UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                var admin = await context.Set<User>().FirstOrDefaultAsync(u => u.Id == "Admin0", cancellationToken);
                if (admin == null)
                {
                    admin = new User
                    {
                        Id = "Admin0",
                        UserName = "Admin",
                        NormalizedUserName = "ADMIN",
                        PasswordHash = new PasswordHasher<User>().HashPassword(null, "AS_AS_s1"),
                        EmailConfirmed = true,
                        Role = RoleEnum.Admin,
                        CreatedAt = DateTime.UtcNow,
                        LockoutEnabled = false,
                        AccessFailedCount = 0,
                        PhoneNumberConfirmed = true,
                    };
                    context.Set<User>().Add(admin);
                    await context.SaveChangesAsync(cancellationToken);
                }
            });

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

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne(u => u.Admin) 
                .HasForeignKey<Admin>(a => a.UserId) 
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Restaurant>()
                .HasOne(R => R.User)
                .WithOne(u => u.Restaurant)
                .HasForeignKey<Restaurant>(R => R.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DeliveryMan>()
                .HasOne(D => D.User)
                .WithOne(u => u.DeliveryMan)
                .HasForeignKey<DeliveryMan>(D => D.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity<Restaurant>()
               .Property(r => r.RestaurantID)
               .ValueGeneratedOnAdd();

            modelBuilder.Entity<Item>()
               .Property(i => i.ItemID)
               .ValueGeneratedOnAdd();

            modelBuilder.Entity<Discount>()
               .Property(d => d.DiscountID)
               .ValueGeneratedOnAdd();

            modelBuilder.Entity<PromoCode>()
               .Property(p => p.PromoCodeID)
               .ValueGeneratedOnAdd();

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
