using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestRoom.Models;
using System.Threading.Tasks;

namespace QuestRoom.Controllers
{
    public class QuestRoomController : Controller
    {
        private readonly QuestRoomContext context;

        public QuestRoomController(QuestRoomContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> QuestRoom(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var room = await context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            return View("QuestRoom", room);
        }

        public async Task<IActionResult> All()
        {
            return View("AllQuestRooms", await context.Rooms.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Description,MinAmountOfPlayers," +
            "Address,TimeOfPassing,MaxAmountOfPlayers,MinAge," +
            "PhoneNumber,Email,Company,Rating,LevelOfFear,LevelOfDifficulty")]
        Room room)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    context.Rooms.Add(room);
                    await context.SaveChangesAsync();
                    return RedirectToAction("/");
                }

            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(room);
        }
    }
}