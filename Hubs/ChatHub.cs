using Microsoft.AspNetCore.SignalR;

namespace ProjetInfo.Hubs
{
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            ConnectedUsers.myConnectedUsers.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ConnectedUsers.myConnectedUsers.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
