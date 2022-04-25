using System.Collections.Generic;

namespace SimpleMessenger.Models
{
    public class User
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<UserConversation> UserConversations { get; set; }
        public string FullName { get { return this.FirstName + " " + this.LastName; } }
    }
}
