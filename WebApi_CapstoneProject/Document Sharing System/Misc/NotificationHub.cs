using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using DSS.Models.DTOs;

namespace DSS.Misc
{
    public class NotificationHub : Hub
    {
        [Authorize]
        public override async Task OnConnectedAsync()
        {
            string? email = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
            string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            Console.WriteLine($"SignalR connected : {email}");

            if (!string.IsNullOrEmpty(email))
            {
                // Put this socket connection into a group named by e-mail
                await Groups.AddToGroupAsync(Context.ConnectionId, email);
                Console.WriteLine($"Added to group : {email}");
            }

            if (!string.IsNullOrEmpty(userId))
            {
                // Also add to a group named by user ID for more specific targeting
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                Console.WriteLine($"Added to user group : {userId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string? email = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
            string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            Console.WriteLine($"SignalR disconnected : {email}");

            if (!string.IsNullOrEmpty(email))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, email);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Method to send notification to specific user by email
        public async Task SendNotificationToUser(string userEmail, NotificationDto notification)
        {
            await Clients.Group(userEmail).SendAsync("ReceiveNotification", notification);
        }

        // Method to send notification to specific user by ID
        public async Task SendNotificationToUserId(string userId, NotificationDto notification)
        {
            await Clients.Group(userId).SendAsync("ReceiveNotification", notification);
        }

        // Method to send notification to all users
        public async Task SendNotificationToAll(NotificationDto notification)
        {
            await Clients.All.SendAsync("ReceiveNotification", notification);
        }

        // Method to send unread count update
        public async Task SendUnreadCountUpdate(string userEmail, int unreadCount)
        {
            await Clients.Group(userEmail).SendAsync("UpdateUnreadCount", unreadCount);
        }

        // Method to send activity update
        public async Task SendActivityUpdate(string userEmail, object activityData)
        {
            await Clients.Group(userEmail).SendAsync("ReceiveActivityUpdate", activityData);
        }
    }
}