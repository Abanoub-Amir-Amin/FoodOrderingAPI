﻿using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Interfaces
{
    public interface IAddressRepo
    {
        public Task<List<Address>> GetAllAddresses(string Username);
        public Task<Address> GetAddress(Guid addressId);
        public Task Add(string Username, AddressDTO address);
        public Task<bool> Update(Guid AddressId,AddressDTO address);
        public Task<bool> Delete(Guid AddressId);
        public Task Save();


    }
}
