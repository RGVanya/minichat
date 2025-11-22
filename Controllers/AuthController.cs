using ChatServer.Data;
using ChatServer.Models;
using ChatServer.Models.ChatServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;


namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ChatDbContext _db;
        public AuthController(ChatDbContext db) => _db = db;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            if (_db.Users.Any(u => u.Username == req.Username))
                return BadRequest("Username already exists");

            var user = new User
            {
                Username = req.Username,
                DisplayName = req.DisplayName,
                PasswordHash = Hash(req.Password)
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Ok(new { user.Id, user.DisplayName, user.Username });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            var user = _db.Users
                .Include(u => u.ReceivedMessages)
                .ThenInclude(m => m.Sender)
                .FirstOrDefault(u => u.Username == req.Username);

            if (user == null || user.PasswordHash != Hash(req.Password))
                return Unauthorized();

            return Ok(new
            {
                user.Id,
                user.Username,
                user.DisplayName
            });
        }

        private string Hash(string pass)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(pass);
            return Convert.ToHexString(sha.ComputeHash(bytes));
        }
    }
}
