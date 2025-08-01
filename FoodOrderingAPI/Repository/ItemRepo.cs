using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Hubs;
using FoodOrderingAPI.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FoodOrderingAPI.Repository
{
    public class ItemRepo : IItemRepo
    {
        public IHubContext<ItemHub> HubContext { get; }
        public ItemRepo(IHubContext<ItemHub> hubContext, ApplicationDBContext dBContext)
        {
            HubContext = hubContext;
        }

        public async Task CreateItemAsync(string restaurantId, ItemDto item)
        {
            await HubContext.Clients.All.SendAsync("ReceiveItem", item);
        }
    }
}
