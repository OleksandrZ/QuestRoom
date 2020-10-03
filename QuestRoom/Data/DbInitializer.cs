using QuestRoom.Models;
using System.Linq;

namespace QuestRoom.Data
{
    public static class DbInitializer
    {
        public static void Initialize(QuestRoomContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Rooms.Any())
            {
                return;   // DB has been seeded
            }

            var rooms = new Room[]
            {
                new Room()
                {
                    Address = "Odessa",
                    Company = "OdessaCompany",
                    Description = "Odessa's room",
                    Email = "odessaroom@gmail.com",
                    LevelOfDifficulty = Difficulty.Normal,
                    LevelOfFear = 1,
                    MinAmountOfPlayers = 1,
                    MaxAmountOfPlayers = 5,
                    MinAge = 16,
                    Name = "OdessaRoom",
                    PhoneNumber = "380 88 888 88 88",
                    Rating = 3.5f,
                    TimeOfPassing = 80
                },
                new Room()
                {
                    Address = "Kyiv",
                    Company = "KyivCompany",
                    Description = "Kyiv's room",
                    Email = "kyivroom@gmail.com",
                    LevelOfDifficulty = Difficulty.Hard,
                    LevelOfFear = 2,
                    MinAmountOfPlayers = 2,
                    MaxAmountOfPlayers = 7,
                    MinAge = 18,
                    Name = "KyivRoom",
                    PhoneNumber = "380 88 888 88 87",
                    Rating = 4.5f,
                    TimeOfPassing = 100
                }
            };

            foreach (var room in rooms)
            {
                context.Rooms.Add(room);
            }
            context.SaveChanges();
        }
    }
}