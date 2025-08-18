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
        IAddressRepo addressRepo;
        IMapper _mapper;
        private readonly IConfiguration _configuration;

        UserManager<User> userManager { get; }

        public CustomerService(ApplicationDBContext dbContext,ICustomerRepo customerRepo,IShoppingCartRepository shoppingCartRepository, IAddressRepo addressRepo,IMapper _mapper, UserManager<User> userManager, IConfiguration configuration)
        {
            _dbContext = dbContext;
            this.customerRepo = customerRepo;
            this.shoppingCartRepo = shoppingCartRepository;
            this._mapper = _mapper;
            this.userManager = userManager;
            this._configuration = configuration;
            this.addressRepo = addressRepo;
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
            Foundcustomer.User.PhoneNumber = customerDto.PhoneNumber;
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
            Console.WriteLine("=== REGISTRATION DEBUG ===");
            Console.WriteLine($"Registering user: {dto.Email}");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            User user = new User();

            // Check for existing email
            if (await IsEmailTaken(dto.Email))
            {
                return IdentityResult.Failed(new IdentityError { Description = "A user with this Email already exists." });
            }

            // Map registration DTO to User entity
            _mapper.Map(dto, user);
            Console.WriteLine($"Mapped user - UserName: {user.UserName}, Email: {user.Email}");

            // Create user with password using Identity
            IdentityResult result = await userManager.CreateAsync(user, dto.Password);
            Console.WriteLine($"User creation result: {result.Succeeded}");

            if (result.Errors.Any())
            {
                Console.WriteLine("User creation errors:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"  - {error.Description}");
                }
            }

            if (result.Succeeded)
            {
                Console.WriteLine($"User created with ID: {user.Id}");

                try
                {
                    // STEP 1: Add Customer Role
                    Console.WriteLine("Adding Customer role...");
                    var roleResult = await userManager.AddToRoleAsync(user, "Customer");
                    Console.WriteLine($"Role assignment result: {roleResult.Succeeded}");

                    if (!roleResult.Succeeded)
                    {
                        Console.WriteLine("Role assignment errors:");
                        foreach (var error in roleResult.Errors)
                        {
                            Console.WriteLine($"  - {error.Description}");
                        }
                    }

                    // Verify role was added
                    var userRoles = await userManager.GetRolesAsync(user);
                    Console.WriteLine($"User roles after assignment: [{string.Join(", ", userRoles)}]");

                    // STEP 2: Create related Customer entity
                    Console.WriteLine("Creating Customer entity...");
                    Customer customer = new Customer
                    {
                        CustomerID = user.Id,
                        UserID = user.Id
                    };
                    _mapper.Map(dto, customer);

                    Console.WriteLine($"Customer - CustomerID: {customer.CustomerID}, UserID: {customer.UserID}");

                    await customerRepo.Add(customer);
                    await customerRepo.Save();
                    Console.WriteLine("Customer entity saved");

                    // STEP 3: Create Shopping Cart
                    Console.WriteLine("Creating shopping cart...");
                    ShoppingCart cart = new ShoppingCart();
                    await shoppingCartRepo.Create(cart, customer.CustomerID);
                    await shoppingCartRepo.Save();
                    Console.WriteLine("Shopping cart created");

                    //add address
                    await addressRepo.Add(user.Id, dto.Address);
                    await addressRepo.Save();
                    Console.WriteLine("Address added");

                    // STEP 5: Commit transaction
                    Console.WriteLine("Committing transaction...");
                    await transaction.CommitAsync();
                    Console.WriteLine("✅ Registration completed successfully");

                    // FINAL VERIFICATION
                    Console.WriteLine("=== FINAL VERIFICATION ===");
                    var verifyUser = await userManager.FindByIdAsync(user.Id);
                    var verifyRoles = await userManager.GetRolesAsync(verifyUser);
                    var verifyCustomer = await customerRepo.GetById(user.Id);

                    Console.WriteLine($"User exists: {verifyUser != null}");
                    Console.WriteLine($"User roles: [{string.Join(", ", verifyRoles)}]");
                    Console.WriteLine($"Customer record exists: {verifyCustomer != null}");
                    Console.WriteLine("=== END VERIFICATION ===");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Registration failed: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    await transaction.RollbackAsync();
                    await userManager.DeleteAsync(user);
                    return IdentityResult.Failed(new IdentityError { Description = $"Registration failed: {ex.Message}" });
                }
            }

            Console.WriteLine("=== END REGISTRATION DEBUG ===");
            return result;
        }


        public async Task Save()
        {
            await customerRepo.Save();
        }

        
    }
}
