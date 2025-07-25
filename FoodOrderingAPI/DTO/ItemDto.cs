﻿namespace FoodOrderingAPI.DTO
{
    public class ItemDto
    {
        public string RestaurantID { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string Category { get; set; }
        public string ImgUrl { get; set; }
    }
}
