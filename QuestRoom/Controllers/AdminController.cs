using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuestRoom.Data;
using QuestRoom.Models;
using QuestRoom.Utilities;
using QuestRoom.ViewModel;

namespace QuestRoom.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly long _fileSizeLimit;

        private readonly string[] _permittedExtensions = { ".jpg", ".gif", ".png" };
        public IWebHostEnvironment HostingEnvironment { get; set; }

        private readonly QuestRoomContext context;

        public AdminController(QuestRoomContext context, IConfiguration config, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            HostingEnvironment = webHostEnvironment;
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
                            var stream = image.OpenReadStream();
                            if (FileHelper.IsValidFileExtensionAndSignature(image.FileName, stream, _permittedExtensions))
                            {
                                if (stream.Length > _fileSizeLimit)
                                {
                                    var megabyteSizeLimit = _fileSizeLimit / 1048576;
                                    ModelState.AddModelError("Files", $"One or more files exceed {megabyteSizeLimit:N1} MB");
                                    return View(room);
                                }
                                string path = Path.Combine(HostingEnvironment.WebRootPath, "images\\" + room.Name);
                                if (!Directory.Exists(path))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(path);
                                }
                                string fullPath = Path.Combine(path, image.FileName);
                                stream = new FileStream(fullPath, FileMode.Create);
                                using (var binaryWriter = new BinaryWriter(stream))
                                {
                                    using (var binaryReader = new BinaryReader(image.OpenReadStream()))
                                    {
                                        binaryWriter.Write(binaryReader.ReadBytes((int)image.Length));
                                    }
                                }
                                var pic = System.Drawing.Image.FromFile(fullPath);
                                img.Name = image.FileName;
                                img.Path = fullPath.Substring(fullPath.IndexOf("images"));
                                img.ContentType = image.ContentType;
                                img.Height = pic.Height;
                                img.Width = pic.Width;
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
                    return Redirect("/");
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
                    return Redirect("/");
                }
                catch (DbUpdateException)
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
                return Redirect("/");
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
    }
}