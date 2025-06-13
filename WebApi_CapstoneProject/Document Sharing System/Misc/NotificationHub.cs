using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DSS.Misc
{
    public class NotificationHub : Hub
    {
        [Authorize]
        public override async Task OnConnectedAsync()
        {

            string? email = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
            Console.WriteLine($"SignalR connected : {email}");

            if (!string.IsNullOrEmpty(email))
            {
                // Put this socket connection into a group named by e-mail
                await Groups.AddToGroupAsync(Context.ConnectionId, email);
                Console.WriteLine($"Added to group : {email}");
            }

            await base.OnConnectedAsync();
        }
    }
}