using ChatServer.Data;
using ChatServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatServer.DTOModule;
using System.Text;
using System.Security.Cryptography;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ChatDbContext _context;
         
        public UsersController(ChatDbContext context)
        {
            _context = context;
        }  
        [HttpPost("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            var user = await _context.Users
                .Include(u => u.ReceivedMessages)
                .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(u => u.Username == request.Login);
            if (user == null)
            {
                return NotFound("Пользователь не найлен");
            }
            if (!string.IsNullOrWhiteSpace(request.NewDisplayName))
            {
                user.DisplayName = request.NewDisplayName;
            }
            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                user.PasswordHash = Hash(request.NewPassword);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var userdto = _context.Users
               .FirstOrDefault(u => u.Username == request.Login);

            return Ok(new
            {
                userdto.Id,
                userdto.Username,
                userdto.DisplayName
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
