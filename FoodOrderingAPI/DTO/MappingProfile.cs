using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map RestaurantDto → Restaurant
        CreateMap<RestaurantDto, Restaurant>()
            // Avoid mapping User.Restaurant to prevent cycles
            .ForMember(dest => dest.User, opt => opt.Ignore());

        // Map Restaurant → RestaurantProfileDto
        CreateMap<Restaurant, RestaurantProfileDto>().ReverseMap(); // Basic ReverseMap for profile updates

        // Map Order → OrderDto
        CreateMap<Order, OrderDto>();

        // Map from Item entity to ItemDto
        CreateMap<Item, ItemDto>();

        // Map DeliveryManDto → DeliveryMan
        CreateMap<DeliveryManDto, DeliveryMan>()
             //Avoid mapping User.DeliveryMan to prevent cycles
            .ForMember(dest => dest.User, opt => opt.Ignore());

        // Map UserDto → User
        CreateMap<UserDto, User>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserID))
              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
              // Prevent EF cycles by ignoring navigation
              .ForMember(u => u.Restaurant, opt => opt.Ignore());

        // Reverse mapping OrderDto → Order
        CreateMap<OrderDto, Order>();

        // Reverse mapping ItemDto → Item
        CreateMap<ItemDto, Item>();

        // Reverse mapping DeliveryMan → DeliveryManDto

        CreateMap<DeliveryMan, DeliveryManDto>();

        // Reverse mapping Restaurant → RestaurantDto
        CreateMap<Restaurant, RestaurantDto>();

        // Reverse mapping RestaurantProfileDto → Restaurant
        CreateMap<RestaurantProfileDto, Restaurant>()
                .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.RestaurantName))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.OpenHours, opt => opt.MapFrom(src => src.OpenHours))
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvailable)) 
                .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
                // Ignore other fields that are not part of the update DTO or should not be updated directly                                                                     
                .ForMember(dest => dest.RestaurantID, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Rating, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore())
                .ForMember(dest => dest.Discounts, opt => opt.Ignore())
                .ForMember(dest => dest.PromoCodes, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore());


        // Map User → UserDto
        CreateMap<User, UserDto>();
    }
}
