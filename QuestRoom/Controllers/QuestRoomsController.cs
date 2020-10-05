using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuestRoom.Models;
using QuestRoom.Utilities;
using QuestRoom.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuestRoom.Controllers
{
    public class QuestRoomsController : Controller
    {
        private readonly QuestRoomContext context;

        //public BufferedMultipleFileUploadDb FileUpload { get; set; }
        private readonly long _fileSizeLimit;

        private readonly string[] _permittedExtensions = { ".jpg", ".gif", ".png" };
        public object Environment { get; private set; }

        public QuestRoomsController(QuestRoomContext context, IConfiguration config)
        {
            this.context = context;
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
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
            var rooms = context.Rooms.Include("Images");
            //var rooms = from room in context.Rooms
            //            select room;

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
            ViewData["CurrentDifficulty"] = difficulties;
            ViewData["CurrentAmount"] = Amount;
            ViewData["CurrentFear"] = Fear;

            int pageSize = 3;
            roomsList = roomsList.OrderBy(r => r.Name).ToList();
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
            "PhoneNumber,Email,Company,Rating,LevelOfFear,LevelOfDifficulty,Files")] RoomViewModel rvm)
        {
            Room room = rvm;
            room.Images = new List<Image>();
            try
            {
                if (ModelState.IsValid)
                {
                    if (rvm.Files.Count > 0)
                    {
                        foreach (var image in rvm.Files)
                        {
                            Image img = new Image();
                            byte[] imageData = null;
                            var stream = image.OpenReadStream();
                            if (FileHelper.IsValidFileExtensionAndSignature(image.FileName, stream, _permittedExtensions))
                            {
                                if(stream.Length > _fileSizeLimit)
                                {
                                    var megabyteSizeLimit = _fileSizeLimit / 1048576;
                                    ModelState.AddModelError("Files", $"One or more files exceed {megabyteSizeLimit:N1} MB");
                                    return View(room);
                                }
                                using (var binaryReader = new BinaryReader(image.OpenReadStream()))
                                {
                                    imageData = binaryReader.ReadBytes((int)image.Length);
                                }
                                img.Name = image.FileName;
                                img.Content = imageData;
                                img.ContentType = image.ContentType;
                                room.Images.Add(img);
                            }
                            else
                            {
                                ModelState.AddModelError("Files", "One or more files have wrong type");
                                return View(room);
                            }
                        }
                    }
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

            var rooms = await context.Rooms.Include("Images").ToListAsync();
            var room = rooms.Find(r => r.Id == id);

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
            var rooms = await context.Rooms.Include("Images").ToListAsync();
            var deleteRoom = rooms.Find(r => r.Id == id);
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