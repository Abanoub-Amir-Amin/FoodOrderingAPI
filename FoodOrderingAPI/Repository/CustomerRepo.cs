using FoodOrderingAPI.Controllers;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Interfaces;
using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace FoodOrderingAPI.Repository
{
    public class CustomerRepo:ICustomerRepo
    {
        ApplicationDBContext dbContext;
        public UserManager<User> UserManager { get; }
        public CustomerRepo(ApplicationDBContext dBContext, UserManager<User> userManager) {
            this.dbContext = dBContext;
            this.UserManager = userManager;
        }
        public async Task<CustomerDTO> GetById(string Customerid)
        {
            Customer? customer = await dbContext.Customers
            .Include(c => c.User)
            .Include(c => c.Addresses)
            .Include(c => c.RewardHistories)
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.CustomerID == Customerid);
            if (customer == null)
                return null;
            CustomerDTO customerDTO = ToDTO(customer);
            return customerDTO;
        }
        public async Task<List<CustomerDTO>> GetAll()
        {
            var customers = await dbContext.Customers
            .Include(c => c.User)
            .Include(c => c.Addresses)
            .Include(c => c.Orders)
            .Include(c => c.RewardHistories)
            .ToListAsync();
            var customerDTOs = customers.Select(c => ToDTO(c)).ToList();
            return customerDTOs;
        }
        public async Task Add(Customer customer)
        {

            await dbContext.Customers.AddAsync(customer);
            //await Save();
        }
        public async Task<bool> Update(string CustomerId, UpdateCustomerDTO customer)
        {
            Customer? Foundcustomer = await dbContext.Customers.Include(c => c.User).FirstOrDefaultAsync(c=> c.CustomerID==CustomerId);

            if (Foundcustomer==null) return false;
            Foundcustomer.FirstName = customer.FirstName;
            Foundcustomer.LastName = customer.LastName;
            Foundcustomer.Gender=customer.Gender;
            Foundcustomer.User.PhoneNumber = customer.Phone;
            //await Save();
            return true;
        }
        public async Task<bool> Delete(string id)
        {
            Customer customer = await dbContext.Customers.FindAsync(id);
            if(customer == null) return false;
            dbContext.Customers.Remove(customer);
            //await Save();
            return true;
        }
        public async Task Save()
        {
            await dbContext.SaveChangesAsync();
        }
        public async Task<CustomerDTO> GetByEmail(string email)
        {
            //return dbContext.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.User.Email==email);
            Customer? customer = await dbContext.Customers
            .Include(c => c.User)
            .Include(c => c.Addresses)
            .Include(c => c.RewardHistories)
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.User.Email == email);

            CustomerDTO customerDTO = ToDTO(customer);
            return customerDTO;
        }
        public async Task<CustomerDTO> GetByUsername(string UserName)
        {
            //return dbContext.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.User.Email==email);
            Customer? customer = await dbContext.Customers
            .Include(c => c.User)
            .Include(c => c.Addresses)
            .Include(c => c.RewardHistories)
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.User.UserName == UserName);

            CustomerDTO customerDTO = ToDTO(customer);
            return customerDTO;
        }
        public async Task<IdentityResult> Register(RegisterCustomerDTO dto)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            User user = new User();
            user.UserName = dto.UserName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Email = dto.Email;
            user.Role = RoleEnum.Customer;
            user.CreatedAt = DateTime.Now;
            IdentityResult result = await UserManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                try
                {
                    Customer customer = new Customer();
                    customer.CustomerID = user.Id;
                    customer.UserID = user.Id;
                    customer.FirstName = dto.FirstName;
                    customer.LastName = dto.LastName;
                    await Add(customer);
                    await Save();
                    #region create shopping cart
                    ShoppingCart cart = new ShoppingCart();
                    cart.CreatedAt = DateTime.Now;
                    cart.CustomerID = customer.CustomerID;
                    await dbContext.ShoppingCarts.AddAsync(cart);
                    await Save();
                    await transaction.CommitAsync();

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    await UserManager.DeleteAsync(user);
                    return IdentityResult.Failed(new IdentityError { Description = "Registration failed while creating customer or shopping cart." });

                }
                #endregion
            }
            return result;
        }
        //public Task<CustomerRepo> Authenticate(LoginDTO customer);
        //public Task<IEnumerable<Order>> GetCustomerOrdersAsync(int customerId);
        //public Task<IEnumerable<Review>> GetCustomerReviewsAsync(int customerId);
        //public Task<IEnumerable<Review>> GetCustomerShoppingCart(int customerId);

        public static CustomerDTO ToDTO(Customer customer)
        {
            return new CustomerDTO
            {
                FirstName = customer.FirstName,
                LastName= customer.LastName,
                UserName = customer.User.UserName,
                Email = customer.User.Email,
                Phone = customer.User.PhoneNumber,
                Gender=customer.Gender,
                Addresses = customer.Addresses
                    .Select(a => $"{a?.Label} - {a?.Street}, {a?.City} (Location: {a?.LatLng})")
                    .ToList(),
                LoyaltyPoints = customer.LoyaltyPoints,
                TotalOrders = customer.Orders.Count(),
                Rewards = customer.RewardHistories.Select(r => r?.Reason).ToList(),
                TotalRewardspoints = customer.RewardHistories.Sum(r => r.PointsEarned)
            };
        }

    }
}
