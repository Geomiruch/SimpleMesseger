using Microsoft.EntityFrameworkCore;
using SimpleMessenger.Models;

namespace SimpleMessenger.Data
{
    public class MessengerContext : DbContext
    {
        public MessengerContext(DbContextOptions<MessengerContext> options) : base(options)
        {
        }

        public DbSet<User> Users{ get; set; }
        public DbSet<Message> Messages{ get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<UserConversation> UserConversations { get; set; }
        public DbSet<ConversationType> ConversationTypes{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Message>().ToTable("Message");
            modelBuilder.Entity<Conversation>().ToTable("Conversation");
            modelBuilder.Entity<ConversationType>().ToTable("ConversationType");

            modelBuilder.Entity<UserConversation>().HasKey(sc => new { sc.UserID, sc.ConversationID});

            modelBuilder.Entity<UserConversation>()
                .HasOne<User>(sc => sc.User)
                .WithMany(s => s.UserConversations)
                .HasForeignKey(sc => sc.UserID);

            modelBuilder.Entity<UserConversation>()
                .HasOne<Conversation>(sc => sc.Conversation)
                .WithMany(s => s.UserConversations)
                .HasForeignKey(sc => sc.ConversationID);
        }
    }
}
