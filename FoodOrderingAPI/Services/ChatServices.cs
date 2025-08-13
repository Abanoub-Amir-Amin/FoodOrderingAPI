using FoodOrderingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Services
{
    public class ChatServices : IChatService
    {
        private readonly ApplicationDBContext _db;

        public ChatServices(ApplicationDBContext db)
        {
            this._db= db;
        }

        public async Task<IEnumerable<Chat>> GetMessagesAsync(string sender, string conversationId)
        {
            return await _db.Chats
                .Where(m => m.ConversationId == conversationId && (m.Sender == sender || m.Sender == "ai" + sender))
                .OrderBy(m => m.TimeStamp)
                .ThenBy(m => m.id)
                .ToListAsync();
        }

        public async Task<Chat> SaveMessageAsync(string sender, string message, string conversationId)
        {
            var chat = new Chat
            {
                Sender = sender,
                Message = message,
                TimeStamp = DateTime.Now,
                ConversationId = conversationId
            };

            await _db.Chats.AddAsync(chat);
            await _db.SaveChangesAsync();
            return chat;
        }

        public async Task<List<string>> GetConversationSessionsAsync(string sender)
        {
            return await _db.Chats
                .Where(c => c.Sender == sender || c.Sender == "ai" + sender)
                .Select(c => c.ConversationId)
                .Distinct()
                .OrderByDescending(c => c)
                .ToListAsync();
        }
    }
}
