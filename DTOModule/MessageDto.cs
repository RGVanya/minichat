namespace ChatServer.DTOModule
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }

       // public string SenderDisplayName {  get; set; }

        public int ReceiverId { get; set; }

      //  public string ReceiverDisplayName { get; set; }

        public string Text { get; set; }

        public DateTime SentAt { get; set; }
    }
}
