using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using bloggit.DTOs;

namespace bloggit.Hubs
{
    public class NotificationHub : Hub<IChatClient>
    {
        public async Task SendMessage(ChatMessage message)
        {
            Console.WriteLine("Message sent");
            await Clients.All.ReceiveMessage(message);
        }
    }
}