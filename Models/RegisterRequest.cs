namespace ChatServer.Models
{
    public class RegisterRequest
    {
        public string Username { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
