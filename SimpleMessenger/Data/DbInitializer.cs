using SimpleMessenger.Models;
using System.Linq;

namespace SimpleMessenger.Data
{
    public static class DbInitializer
    {
        public static void Initialize(MessengerContext context)
        {

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            
            // Look for any Users.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            var users = new User[]
            {
                new User{FirstName="Carson",LastName="Alexander", Email="1@gmail.com", Password="1@gmail.com"},
                new User{FirstName="Meredith", LastName="Alonso",Email="2@gmail.com", Password="2@gmail.com"},
                new User{FirstName="Arturo", LastName="Anand",Email="3@gmail.com", Password="3@gmail.com"},
                new User{FirstName="Gytis", LastName="Barzdukas",Email="4@gmail.com", Password="4@gmail.com"},
                new User{FirstName="Yan", LastName="Li",Email="5@gmail.com", Password="5@gmail.com"},
                new User{FirstName="Peggy",LastName="Justice",Email="6@gmail.com", Password="6@gmail.com"},
                new User{FirstName="Laura",LastName="Norman", Email="7@gmail.com", Password="7@gmail.com"},
                new User{FirstName="Nino",LastName="Olivetto",Email="8@gmail.com", Password="8@gmail.com"}
            };
            
            foreach (User u in users)
            {
                context.Users.Add(u);
            }

            context.SaveChanges();

            var types = new ConversationType[]
            {
                new ConversationType{Name = "Личная переписка"},
                new ConversationType{Name = "Групповой чат"}
            };

            foreach (ConversationType c in types)
            {
                context.ConversationTypes.Add(c);
            }

            context.SaveChanges();
        }
    }
}
