using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Interfaces;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Services
{
    public class CustomerService:ICustomerServices
    {
        ApplicationDBContext _dbContext;
        ICustomerRepo customerRepo;
        IShoppingCartRepository shoppingCartRepo;
        IMapper _mapper;
        UserManager<User> userManager { get; }

        public CustomerService(ApplicationDBContext dbContext,ICustomerRepo customerRepo,IShoppingCartRepository shoppingCartRepository, IMapper _mapper, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            this.customerRepo = customerRepo;
            this.shoppingCartRepo = shoppingCartRepository;
            this._mapper = _mapper;
            this.userManager = userManager;
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
            if(await IsEmailTaken(dto.Email))
            {
                return IdentityResult.Failed(new IdentityError { Description = "A user with this Email already exists." });

            }
            _mapper.Map(dto, user);
            //user.UserName = dto.UserName;
            //user.PhoneNumber = dto.Phone;
            //user.Email = dto.Email;
            //user.Role = RoleEnum.Customer;
            //user.CreatedAt = DateTime.Now;
            IdentityResult result = await userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                try
                {
                    Customer customer = new Customer();
                    customer.CustomerID = user.Id;
                    customer.UserID = user.Id;
                    _mapper.Map(dto, customer);
                    await customerRepo.Add(customer);
                    await customerRepo.Save();
                    #region create shopping cart
                    ShoppingCart cart = new ShoppingCart();
                    await shoppingCartRepo.Create(cart,customer.CustomerID);
                    await shoppingCartRepo.Save();
                    await transaction.CommitAsync();

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    await userManager.DeleteAsync(user);
                    return IdentityResult.Failed(new IdentityError { Description = "Registration failed while creating customer or shopping cart." });

                }
                #endregion
            }
            return result;
        }
        public async Task Save()
        {
            await customerRepo.Save();
        }

        
    }
}
