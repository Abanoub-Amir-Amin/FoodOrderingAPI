using FoodOrderingAPI.DTO;
using Microsoft.AspNetCore.SignalR;

namespace FoodOrderingAPI.Hubs
{
    public class ItemHub:Hub
    {
        public async Task PublishItem(ItemDto item)
        {
            await Clients.All.SendAsync("ReceiveItem", item);
        }
    }
}
