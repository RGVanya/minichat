namespace ChatServer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = "";
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";

        public List<Message> SentMessages { get; set; } = new();
        public List<Message> ReceivedMessages { get; set; } = new();

        public List<UserContact> Contacts { get; set; } = new();
    }

}
