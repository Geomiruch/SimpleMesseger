using System;

namespace SimpleMessenger.Models
{
    public class Message
    {
        public int ID { get; set; }
        public string Body { get; set; }
        public DateTime Created { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public int ConversationID { get; set; }
        public string Reply { get; set; }
        public Conversation Conversation { get; set; }
        //if our message delete only for us...
        public Boolean IsActive { get; set; }
    }
}
