using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestRoom.Models
{
    public class QuestRoomContext: DbContext
    {
        protected QuestRoomContext() : base()
        {
        }

        public QuestRoomContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Room> Rooms { get; set; }
        //public DbSet<Image> Images { get; set; }
    }
}
