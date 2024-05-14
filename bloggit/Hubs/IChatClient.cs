using bloggit.DTOs;

namespace bloggit.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(ChatMessage message);
}