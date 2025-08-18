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
        //public UserManager<User> UserManager { get; }
        public CustomerRepo(ApplicationDBContext dBContext) {
            this.dbContext = dBContext;
        }
        public async Task<Customer> GetById(string Customerid)
        {
            Customer? customer = await dbContext.Customers
            .Include(c => c.User)
            .Include(c => c.Addresses)
            //.Include(c => c.RewardHistories)
            .Include(c => c.Orders)
            .ThenInclude(O => O.OrderItems)
            .ThenInclude(OI => OI.Item)
            .FirstOrDefaultAsync(c => c.CustomerID == Customerid);
            return customer;
        }
        public async Task<List<Customer>> GetAll()
        {
            var customers = await dbContext.Customers
            .Include(c => c.User)
            .Include(c => c.Addresses)
            .Include(c => c.Orders)
            //.Include(c => c.RewardHistories)
            .ToListAsync();
            return customers;
        }
        public async Task Add(Customer customer)
        {

            await dbContext.Customers.AddAsync(customer);
            //await Save();
        }
        public async Task<Customer> Update(Customer customer)
        {
            dbContext.Users.Update(customer.User);
            dbContext.Customers.Update(customer);
            return customer;
        }
        public async Task<Customer> Delete(string id)
        {
            Customer customer = await dbContext.Customers.FindAsync(id);
            dbContext.Customers.Remove(customer);
            //await Save();
            return customer;
        }
        public async Task Save()
        {
            await dbContext.SaveChangesAsync();
        }
        public async Task<Customer> GetByEmail(string email)
        {
            //return dbContext.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.User.Email==email);
            Customer? customer = await dbContext.Customers
            .Include(c => c.User)
            .Include(c => c.Addresses)
            //.Include(c => c.RewardHistories)
            .Include(c => c.Orders)
            .ThenInclude(O => O.OrderItems)
            .ThenInclude(OI => OI.Item)
            .FirstOrDefaultAsync(c => c.User.Email == email);
            return customer;
        }
        public async Task<Customer> GetByUsername(string UserName)
        {
            //return dbContext.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.User.Email==email);
            Customer? customer = await dbContext.Customers
            .Include(c => c.User)
            .Include(c => c.Addresses)
            //.Include(c => c.RewardHistories)
           .Include(c => c.Orders)
           .ThenInclude(O => O.OrderItems)
           .ThenInclude(OI => OI.Item)
           .FirstOrDefaultAsync(c => c.User.UserName == UserName);
            return customer;
        }
        //public async Task<IdentityResult> Register(RegisterCustomerDTO dto)
        //{
        //    using var transaction = await dbContext.Database.BeginTransactionAsync();
        //    User user = new User();
        //    user.UserName = dto.UserName;
        //    user.PhoneNumber = dto.Phone;
        //    user.Email = dto.Email;
        //    user.Role = RoleEnum.Customer;
        //    user.CreatedAt = DateTime.Now;
        //    IdentityResult result = await UserManager.CreateAsync(user, dto.Password);
        //    if (result.Succeeded)
        //    {
        //        try
        //        {
        //            Customer customer = new Customer();
        //            customer.CustomerID = user.Id;
        //            customer.UserID = user.Id;
        //            customer.FirstName = dto.FirstName;
        //            customer.LastName = dto.LastName;
        //            await Add(customer);
        //            await Save();
        //            #region create shopping cart
        //            ShoppingCart cart = new ShoppingCart();
        //            cart.CreatedAt = DateTime.Now;
        //            cart.CustomerID = customer.CustomerID;
        //            await dbContext.ShoppingCarts.AddAsync(cart);
        //            await Save();
        //            await transaction.CommitAsync();

        //        }
        //        catch (Exception ex)
        //        {
        //            await transaction.RollbackAsync();
        //            await UserManager.DeleteAsync(user);
        //            return IdentityResult.Failed(new IdentityError { Description = "Registration failed while creating customer or shopping cart." });

        //        }
        //        #endregion
        //    }
        //    return result;
        //}
        //public Task<CustomerRepo> Authenticate(LoginDTO customer);
        //public Task<IEnumerable<Order>> GetCustomerOrdersAsync(int customerId);
        //public Task<IEnumerable<Review>> GetCustomerReviewsAsync(int customerId);
        //public Task<IEnumerable<Review>> GetCustomerShoppingCart(int customerId);

        

    }
}
