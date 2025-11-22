namespace ChatServer.Models
{
    public class UpdateUserRequest
    {
        public string Login { get; set; }      
        public string? NewDisplayName { get; set; }
        public string? NewPassword { get; set; }
    }
}
