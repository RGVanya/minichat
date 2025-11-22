using ChatServer.Data;
using ChatServer.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatServer.Services
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<int, string> onlineUsers = new();

        private readonly ChatDbContext _db;
        public ChatHub(ChatDbContext db) => _db = db;

        public override async Task OnDisconnectedAsync(Exception? e)
        {
            var user = onlineUsers.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (user.Key != 0)
                onlineUsers.Remove(user.Key);

            await base.OnDisconnectedAsync(e);
        }

        public Task RegisterOnline(int userId)
        {
            onlineUsers[userId] = Context.ConnectionId;
            return Task.CompletedTask;
        }

        public async Task SendMessage(int fromId, int toId, string text, DateTime dateTime)
        {
            var msg = new Message
            {
                SenderId = fromId,
                ReceiverId = toId,
                Text = text,
                SentAt = dateTime,
                IsDelivered = onlineUsers.ContainsKey(toId)
            };

            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();

            if (onlineUsers.TryGetValue(toId, out var connId))
            {
                await Clients.Client(connId).SendAsync("ReceiveMessage", msg.Text, msg.SenderId, msg.SentAt);

            }

            
        }
    }

}
