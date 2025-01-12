using Microsoft.AspNetCore.SignalR;
using TasksManagementServer.Models;
namespace TasksManagementServer.Hubs
{
    public class ChatHub : Hub
    {
        private static Dictionary<string , string> connectedUsers = new Dictionary<string, string>();
        private readonly TasksManagementDbContext dbContext;
        public ChatHub(TasksManagementDbContext dbContext) 
        {
            this.dbContext = dbContext;
        }
        
        
        public async Task SendMessage(string userId, string message)
        {
            //Find all connections for the user id who need to recieve the message
            List<KeyValuePair<string, string>>? connections = connectedUsers.Where(x => x.Value == userId).ToList();
            //Find the user ID of the sender based on its connection id
            string? sender = connectedUsers[Context.ConnectionId];
            //If all is good, loop through the connections and send them all the message
            if (connections != null && sender != null)
            {
                foreach (KeyValuePair<string,string> connection in connections)
                {
                    await Clients.Client(connection.Key).SendAsync("ReceiveMessage", sender, message);
                }
            }
        }

        public async Task OnConnect(string userId)
        {
            connectedUsers.Add(Context.ConnectionId, userId);
            await base.OnConnectedAsync();
        }

        public async Task OnDisconnect()
        {
            connectedUsers.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(null);
        }
    }
}