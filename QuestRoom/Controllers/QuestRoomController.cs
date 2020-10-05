using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
<<<<<<< Updated upstream
using QuestRoom.Models;
using System.Collections.Generic;
=======
using Microsoft.Extensions.Configuration;
using QuestRoom.Models;
using QuestRoom.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
>>>>>>> Stashed changes
using System.Linq;
using System.Threading.Tasks;

namespace QuestRoom.Controllers
{
    public class QuestRoomController : Controller
    {
        private readonly QuestRoomContext context;
<<<<<<< Updated upstream

        public QuestRoomController(QuestRoomContext context)
=======

        //public BufferedMultipleFileUploadDb FileUpload { get; set; }
        private readonly long _fileSizeLimit;
        private readonly string[] _permittedExtensions = { ".txt", ".jpg" };

        public object Environment { get; private set; }

        public QuestRoomController(QuestRoomContext context, IConfiguration config)
>>>>>>> Stashed changes
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

        public async Task<IActionResult> Index(string SearchString, string[] difficulties, string[] Amount, string[] Fear, int? page)
        {
            if (SearchString != null)
            {
                page = 1;
            }

            var rooms = from room in context.Rooms
                        select room;

            List<Room> roomsList;
            if (!string.IsNullOrEmpty(SearchString))
            {
                rooms = rooms.Where(r => r.Name.Contains(SearchString));
                roomsList = await rooms.ToListAsync();
            }
            else
            {
                roomsList = await rooms.ToListAsync();
                if (difficulties.Length > 0)
                {
                    roomsList = roomsList.Where(r => difficulties.Contains(r.LevelOfDifficulty.ToString())).ToList();
                }
                if (Amount.Length > 0)
                {
                    int min = int.Parse(Amount[0]);
                    int max = int.Parse(Amount[1]);

                    roomsList = roomsList.Where(r => r.MinAmountOfPlayers >= min && r.MaxAmountOfPlayers <= max).ToList();
                }
                if (Fear.Length > 0)
                {
                    roomsList = roomsList.Where(r => Fear.Contains(r.LevelOfFear.ToString())).ToList();
                }
            }
            ViewData["CurrentSearch"] = SearchString;
            ViewData["CurrentDifficulty"] = difficulties.ToList();
            ViewData["CurrentAmount"] = Amount.ToList();
            ViewData["CurrentFear"] = Fear.ToList();

            int pageSize = 3;
            return View(PaginatedList<Room>.Create(roomsList, page ?? 1, pageSize));
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
<<<<<<< Updated upstream
            "PhoneNumber,Email,Company,Rating,LevelOfFear,LevelOfDifficulty")] Room room)
=======
            "PhoneNumber,Email,Company,Rating,LevelOfFear,LevelOfDifficulty,Files")] RoomViewModel rvm)
>>>>>>> Stashed changes
        {
            Room room = rvm;
            room.Images = new List<Image>();
            //Room room = new Room();
            try
            {
                if (ModelState.IsValid)
                {
<<<<<<< Updated upstream
=======
                    if (rvm.Files.Count > 0)
                    {
                        foreach (var image in rvm.Files)
                        {
                            Image img = new Image();
                            byte[] imageData = null;
                            using (var binaryReader = new BinaryReader(image.OpenReadStream()))
                            {
                                imageData = binaryReader.ReadBytes((int)image.Length);
                            }
                            img.Name = image.FileName;
                            img.Content = imageData;
                            img.Extension = image.ContentType;
                            room.Images.Add(img);
                        }
                    }
>>>>>>> Stashed changes
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