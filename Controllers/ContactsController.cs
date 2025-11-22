using ChatServer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatServer.DTOModule;
using ChatServer.Models;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly ChatDbContext _context;
        public ContactsController(ChatDbContext context)
        {
            _context = context;
        }

        [HttpPost("add-contact")]
        public async Task<IActionResult> AddContact([FromBody] AddContactRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.ContactLogin))
                return BadRequest(new { Message = "Оба логина должны быть указаны." });

            if (request.UserName == request.ContactLogin)
                return BadRequest(new { Message = "Нельзя добавить самого себя в контакты." });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.UserName);
            var contact = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.ContactLogin);

            if (user == null || contact == null)
                return NotFound(new { Message = "Пользователь или контакт не найдены." });

            bool alreadyExists = await _context.UserContacts
                .AnyAsync(uc => uc.UserId == user.Id && uc.ContactId == contact.Id);

            if (alreadyExists)
                return Conflict(new { Message = "Контакт уже добавлен." });

            var userContact = new UserContact
            {
                UserId = user.Id,
                ContactId = contact.Id
            };

            _context.UserContacts.Add(userContact);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Контакт успешно добавлен." });
        }


        [HttpGet]
        public async Task<ActionResult<List<ContactDto>>> GetContacts([FromQuery] int userId)
        {
            var contacts = await _context.UserContacts
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.Contact)
                .Select(uc => new ContactDto
                {
                    Id = uc.Contact.Id,
                    Username = uc.Contact.Username,
                    DisplayName = uc.Contact.DisplayName
                })
                .ToListAsync();

            return Ok(contacts);
        }

        [HttpPost("remove-contact")]
        public async Task<IActionResult> RemoveContact([FromBody] RemoveContactRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.UserLogin);
            var contact = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.ContactLogin);

            if (user == null || contact == null)
                return NotFound("Один из пользователей не найден.");

            
            var userContact = await _context.UserContacts
                .FirstOrDefaultAsync(uc => uc.UserId == user.Id && uc.ContactId == contact.Id);

            if (userContact != null)
            {
                _context.UserContacts.Remove(userContact);
            }

            var messages = _context.Messages.Where(m =>
                (m.SenderId == user.Id && m.ReceiverId == contact.Id) ||
                (m.SenderId == contact.Id && m.ReceiverId == user.Id));

            _context.Messages.RemoveRange(messages);

            await _context.SaveChangesAsync();

            return Ok("Контакт и история сообщений удалены.");
        }

    }
}
