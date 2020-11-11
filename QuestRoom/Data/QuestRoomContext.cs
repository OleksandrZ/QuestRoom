using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuestRoom.Areas.Identity.Data;
using QuestRoom.Models;

namespace QuestRoom.Data
{
    public class QuestRoomContext : IdentityDbContext<QuestRoomUser>
    {
        public QuestRoomContext(DbContextOptions<QuestRoomContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        public DbSet<Room> Rooms { get; set; }
    }
}
