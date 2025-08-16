using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FoodOrderingAPI.DTO
{
    public class ItemUpdateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string Category { get; set; }

        // The file being uploaded for the item's image
        public IFormFile? ImageFile { get; set; }

        [BindNever]
        public string? ImageUrl { get; set; } // Optional URL to update the image without uploading a new file
    }
}
