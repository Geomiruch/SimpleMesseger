using System.Collections.Generic;

namespace SimpleMessenger.Models
{
    public class ConversationType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ICollection<Conversation> Conversations { get; set; }
    }
}
