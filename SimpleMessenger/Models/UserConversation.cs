namespace SimpleMessenger.Models
{
    public class UserConversation
    {
        public int UserID { get; set; }
        public User User { get; set; }
        public int ConversationID { get; set; }
        public Conversation Conversation { get; set; }  
    }
}
