using AutoMapper;
using Azure.Core;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Interfaces;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;

namespace FoodOrderingAPI.Services
{
    public class CustomerService:ICustomerServices
    {
        ApplicationDBContext _dbContext;
        ICustomerRepo customerRepo;
        IShoppingCartRepository shoppingCartRepo;
        IMapper _mapper;
        private readonly IConfiguration _configuration;

        UserManager<User> userManager { get; }

        public CustomerService(ApplicationDBContext dbContext,ICustomerRepo customerRepo,IShoppingCartRepository shoppingCartRepository, IMapper _mapper, UserManager<User> userManager, IConfiguration configuration)
        {
            _dbContext = dbContext;
            this.customerRepo = customerRepo;
            this.shoppingCartRepo = shoppingCartRepository;
            this._mapper = _mapper;
            this.userManager = userManager;
            this._configuration = configuration;
        }

        public async Task<CustomerDTO> GetCusomerDashboardDataById(string id) 
        {
            if (id == string.Empty)
            {
                throw new ArgumentNullException("invalid customer id",nameof(id));
            }
            Customer customer = await customerRepo.GetById(id);
            if (customer == null)
                return null;
            CustomerDTO customerDTO=_mapper.Map<CustomerDTO>(customer);
            return customerDTO;
        }
        public async Task<List<CustomerDTO>> GetAll()
        {
            List<Customer> customers = await customerRepo.GetAll();
            var customerDTOs = customers.Select(c => _mapper.Map<CustomerDTO>(c)).ToList();
            return customerDTOs;
        }
        public async Task<bool> UpdateCustomer(string id, UpdateCustomerDTO customerDto)

        { 
            Customer? Foundcustomer = await customerRepo.GetById(id);

            if (Foundcustomer == null) return false;
            //Foundcustomer.FirstName = customerDto.FirstName;
            //Foundcustomer.LastName = customerDto.LastName;
            //Foundcustomer.Gender = customerDto.Gender;
            //if(!string.IsNullOrWhiteSpace(customerDto.Phone))
            //    Foundcustomer.User.PhoneNumber = customerDto.Phone;
            _mapper.Map(customerDto, Foundcustomer);
            customerRepo.Update(Foundcustomer);
            return true;
        }
        //public async Task ChangeEmail(string id,string newemail,string oldpassword)
        //{

        //}
        public async Task<bool> DeleteCustomer(string id)
        {
            Customer customer = await customerRepo.GetById(id);
            if (customer == null) return false;
            Customer DeletedCustomer= await customerRepo.Delete(id);
            //await Save();
            return true;
        }
        public async Task<CustomerDTO> GetCusomerDashboardDataByEmail(string email)
        {
            if (email == string.Empty)
            {
                throw new ArgumentNullException("invalid customer email", nameof(email));
            }
            Customer customer = await customerRepo.GetByEmail(email);
            if (customer == null)
                return null;
            CustomerDTO customerDTO = _mapper.Map<CustomerDTO>(customer);
            return customerDTO;
        }
        public async Task<CustomerDTO> GetCusomerDashboardDataByUserName(string UserName)
        {
            if (UserName == string.Empty)
            {
                throw new ArgumentNullException("invalid customer UserName", nameof(UserName));
            }
            Customer customer = await customerRepo.GetByUsername(UserName);
            if (customer == null)
                return null;
            CustomerDTO customerDTO = _mapper.Map<CustomerDTO>(customer);
            return customerDTO;
        }
        public async Task<bool> IsEmailTaken(string email)
        {
            return await userManager.FindByEmailAsync(email)==null?false:true;
        }
        public async Task<IdentityResult> Register(RegisterCustomerDTO dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            User user = new User();

            // Check for existing email
            if (await IsEmailTaken(dto.Email))
            {
                return IdentityResult.Failed(new IdentityError { Description = "A user with this Email already exists." });
            }

            // Map registration DTO to User entity
            _mapper.Map(dto, user);

            // Create user with password using Identity
            IdentityResult result = await userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                try
                {
                    // Create related Customer entity
                    Customer customer = new Customer
                    {
                        CustomerID = user.Id,
                        UserID = user.Id
                    };
                    _mapper.Map(dto, customer);

                    await customerRepo.Add(customer);
                    await customerRepo.Save();

                    // Create Shopping Cart
                    ShoppingCart cart = new ShoppingCart();
                    await shoppingCartRepo.Create(cart, customer.CustomerID);
                    await shoppingCartRepo.Save();

                    // Commit transaction if everything succeeded
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    await userManager.DeleteAsync(user);
                    return IdentityResult.Failed(new IdentityError { Description = $"Registration failed: {ex.Message}" });
                }
            }
            return result;
        }

        
        public async Task Save()
        {
            await customerRepo.Save();
        }

        
    }
}
