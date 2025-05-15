using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class RideHub : Hub
{
    public Task JoinRideGroup(int rideId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, $"ride-{rideId}");
    }

    // Call this from your backend when the driver accepts the ride:
    public async Task NotifyRideStatus(int rideId, string status)
    {
        await Clients.Group($"ride-{rideId}").SendAsync("ReceiveRideStatus", status);
    }
}
