namespace ChatServer.Models
{
    public class RemoveContactRequest
    {
        public string UserLogin { get; set; }
        public string ContactLogin { get; set; }
    }
}
