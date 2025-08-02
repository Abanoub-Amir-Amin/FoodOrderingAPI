using FoodOrderingAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderingAPI.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Compare("Email")]
        public string EmailConfirmation { get; set; }//leh email confirmation
        public string? Phone { get; set; }
        //[EnumDataType(typeof(RoleEnum))]
        //public RoleEnum Role { get; set; }
        //will not enter his  in register
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        //public bool AgreeTerms {  get; set; }
    }
}
