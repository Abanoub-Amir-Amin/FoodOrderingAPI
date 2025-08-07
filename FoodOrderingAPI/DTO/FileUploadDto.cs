using Swashbuckle.AspNetCore.Annotations;

namespace FoodOrderingAPI.DTO
{
    public class FileUploadDto
    {
        [SwaggerSchema("Upload file")]
        public IFormFile File { get; set; }
    }

}
