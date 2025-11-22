namespace ChatServer.Models
{
    public class UserContact
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int ContactId { get; set; }
        public User Contact { get; set; }
    }
}
