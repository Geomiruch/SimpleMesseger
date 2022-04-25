using System.Collections.Generic;

namespace SimpleMessenger.Models
{
    public class Conversation
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ConversationTypeID { get; set; } 
        public ConversationType ConversationType { get; set; }
        public ICollection<UserConversation> UserConversations { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
