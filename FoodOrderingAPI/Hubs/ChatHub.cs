using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace FoodOrderingAPI.Hubs
{
    public class ChatHub : Hub
    {
        public ApplicationDBContext DBContext { get; }
        public UserManager<User> UserManager { get; }

        public ChatHub(ApplicationDBContext dBContext, UserManager<User> userManager)
        {
            DBContext = dBContext;
            UserManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("=== ChatHub OnConnectedAsync Called ===");

            // Debug authentication
            Console.WriteLine($"Context.User.Identity.IsAuthenticated: {Context.User?.Identity?.IsAuthenticated}");
            Console.WriteLine($"Context.UserIdentifier: {Context.UserIdentifier}");
            Console.WriteLine($"Connection ID: {Context.ConnectionId}");

            var id = Context.UserIdentifier;
            if (id == null)
            {
                Console.WriteLine("❌ User not authenticated - no UserIdentifier");
                return;
            }

            Console.WriteLine($"✅ User {id} connecting to ChatHub");

            try
            {
                var userConnectionId = new User_ConnectionId
                {
                    UserId = id,
                    ConnectionId = Context.ConnectionId
                };

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    Console.WriteLine($"❌ User {id} not found in UserManager");
                    return;
                }

                var roles = await UserManager.GetRolesAsync(user);
                Console.WriteLine($"User {id} roles: [{string.Join(", ", roles)}]");

                DBContext.User_ConnectionId.Add(userConnectionId);

                // Check if user is a customer
                bool hasCustomerRole = roles.Contains("Customer");
                bool hasCustomerRecord = DBContext.Customers.Any(c => c.UserID == id);
                bool isCustomer = hasCustomerRole || hasCustomerRecord;

                Console.WriteLine($"Has Customer role: {hasCustomerRole}");
                Console.WriteLine($"Has Customer record: {hasCustomerRecord}");
                Console.WriteLine($"Is Customer: {isCustomer}");

                if (isCustomer)
                {
                    var chatFound = DBContext.ComplaintChats.FirstOrDefault(c => c.CustomerID == id);
                    Console.WriteLine($"Existing chat found: {chatFound != null}");

                    if (chatFound == null)
                    {
                        var admin = DBContext.Admins.FirstOrDefault();
                        if (admin == null)
                        {
                            Console.WriteLine("❌ No admin found in database!");
                        }
                        else
                        {
                            Console.WriteLine($"Creating new chat - Admin ID: {admin.AdminID}");
                            var userChat = new ComplaintChat
                            {
                                AdminID = admin.AdminID,
                                StartedAt = DateTime.UtcNow,
                                CustomerID = id,
                            };
                            DBContext.ComplaintChats.Add(userChat);
                            Console.WriteLine($"✅ Created new chat for customer {id}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"✅ Using existing chat ID: {chatFound.ChatID}");
                    }
                }

                await DBContext.SaveChangesAsync();
                Console.WriteLine("✅ Database changes saved successfully");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in OnConnectedAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            await base.OnConnectedAsync();
            Console.WriteLine("=== ChatHub OnConnectedAsync Completed ===");
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"=== ChatHub OnDisconnectedAsync - User: {Context.UserIdentifier} ===");

            var id = Context.UserIdentifier;
            if (id != null)
            {
                var row = DBContext.User_ConnectionId.Find(id, Context.ConnectionId);
                if (row != null)
                {
                    DBContext.Remove(row);
                    DBContext.SaveChanges();
                    Console.WriteLine($"✅ Removed connection for user {id}");
                }
            }

            return Task.CompletedTask;
        }
    }
}
