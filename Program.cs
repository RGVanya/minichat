using ChatServer.Data;
using ChatServer.Services;
using Microsoft.EntityFrameworkCore;
 

namespace ChatServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ChatDbContext>(options =>
                options.UseSqlite("Data Source=chat.db"));

            builder.Services.AddSignalR();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseRouting();
            app.MapControllers();
            app.MapHub<ChatHub>("/chathub");


            app.UseSwagger();
            app.UseSwaggerUI();

            app.Run();
        }
    }
}
