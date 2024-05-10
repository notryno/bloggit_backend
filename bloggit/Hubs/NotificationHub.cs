using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace bloggit.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            Console.WriteLine("Message sent");
            await Clients.All.SendAsync("ReceiveNotification", user, message);
        }
    }
}