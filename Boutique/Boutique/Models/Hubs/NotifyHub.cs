using Microsoft.AspNet.SignalR;

namespace Boutique.Models.Hubs
{
    public class NotifyHub : Hub
    {
        public void NotifyOrderPlaced(string orderId)
        {
            Clients.All.orderPlaced(orderId);
        }
    }
}