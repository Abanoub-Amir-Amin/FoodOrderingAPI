using FoodOrderingAPI.Models;
using System.Collections;

namespace FoodOrderingAPI.Services
{
    public interface IChatService
    {
        Task<Chat> SaveMessageAsync(string sender, string message, string conversationId);
        Task<IEnumerable<Chat>> GetMessagesAsync(string sender, string conversationId);

        Task<List<string>> GetConversationSessionsAsync(string sender);
    }
}
