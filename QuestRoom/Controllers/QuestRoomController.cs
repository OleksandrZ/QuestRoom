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

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var room = await context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        public async Task<IActionResult> Index()
        {
            return View(await context.Rooms.ToListAsync());
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
            "PhoneNumber,Email,Company,Rating,LevelOfFear,LevelOfDifficulty")] Room room)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    context.Rooms.Add(room);
                    await context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var updateRoom = await context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (await TryUpdateModelAsync<Room>(updateRoom, "",
                r => r.Name,
                r => r.Description,
                r => r.Address,
                r => r.MinAmountOfPlayers,
                r => r.TimeOfPassing,
                r => r.MaxAmountOfPlayers,
                r => r.MinAge,
                r => r.PhoneNumber,
                r => r.Email,
                r => r.Company,
                r => r.Rating,
                r => r.LevelOfFear,
                r => r.LevelOfDifficulty))
            {
                try
                {
                    await context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(updateRoom);
        }

        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var deleteRoom = await context.Rooms.FindAsync(id);
            if (deleteRoom == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                context.Remove(deleteRoom);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
    }
}