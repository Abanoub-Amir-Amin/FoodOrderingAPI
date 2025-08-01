using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map OrderItem → OrderItemDto
        CreateMap<OrderItem, OrderItemDto>();

        // Map Admin → AdminDto
        CreateMap<Admin, AdminDto>();

        // Map Customer → CustomerDTO
        CreateMap<Customer, CustomerDTO>()
            .ForMember(dest => dest.InProcessOrders, opt => opt.MapFrom(src => src.Orders.Where(o => o.Status == "In Process")));

        // Map Restaurant → RestaurantDto
        CreateMap<Restaurant, RestaurantDto>();

        // Map Order → OrderDto
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));


        // Map Item to ItemDto
        CreateMap<Item, ItemDto>()
            .ForMember(dest => dest.ImageFile, opt => opt.MapFrom(src => src.ImageFile));

        // Map DeliveryManDto → DeliveryMan
        CreateMap<DeliveryManDto, DeliveryMan>()
             //Avoid mapping User.DeliveryMan to prevent cycles
            .ForMember(dest => dest.User, opt => opt.Ignore());

        // Map mapping PromoCodeDto → PromoCode
        CreateMap<PromoCodeDto, PromoCode>()//PromoCodeID
            .ForMember(dest => dest.PromoCodeID, opt => opt.Ignore());

        // Map UserDto → User
        CreateMap<UserDto, User>()
              //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserID))
              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
              // Prevent EF cycles by ignoring navigation
              .ForMember(u => u.Restaurant, opt => opt.Ignore());

        // Reverse mapping OrderDto → Order
        CreateMap<OrderDto, Order>();

        // Reverse mapping AdminDto → Admin
        CreateMap<AdminDto, Admin>();

        // Reverse mapping ItemDto to Item
        CreateMap<ItemDto, Item>()
            .ForMember(dest => dest.ImageFile, opt => opt.MapFrom(src => src.ImageFile))
            // Ignore keys or IDs assigned explicitly
            .ForMember(dest => dest.ItemID, opt => opt.Ignore())
            .ForMember(dest => dest.RestaurantID, opt => opt.Ignore());

        // Reverse Mapping RestaurantDto → Restaurant
        CreateMap<RestaurantDto, Restaurant>()
                // Ignore other fields that are not part of the update DTO or should not be updated directly                                                                     
                .ForMember(dest => dest.RestaurantID, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Rating, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore())
                .ForMember(dest => dest.Discounts, opt => opt.Ignore())
                .ForMember(dest => dest.PromoCodes, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore());

        // Reverse mapping DeliveryMan → DeliveryManDto
        CreateMap<DeliveryMan, DeliveryManDto>();

        // Reverse mapping PromoCode → PromoCodeDto
        CreateMap<PromoCode, PromoCodeDto>()
            .ForMember(dest => dest.PromoCodeID, opt => opt.Ignore());

        // Reverse mapping Restaurant → RestaurantDto
        CreateMap<Restaurant, RestaurantDto>();

        // Map User → UserDto
        CreateMap<User, UserDto>();
    }
}
