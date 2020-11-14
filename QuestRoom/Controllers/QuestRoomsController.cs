using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestRoom.Data;
using QuestRoom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestRoom.Controllers
{
    public class QuestRoomsController : Controller
    {
        private readonly QuestRoomContext context;

        //public BufferedMultipleFileUploadDb FileUpload { get; set; }

        public QuestRoomsController(QuestRoomContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var room = await context.Rooms.Include("Images").FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }
        //Отображение страници со всеми комнатами с возможностью фильтрации, многостраничности и поиска
        public async Task<IActionResult> Index(string SearchString, string[] difficulties, string[] Amount, string[] Fear, int? page)
        {
            if (SearchString != null)
            {
                page = 1;
            }
            var rooms = context.Rooms.Include("Images");

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
    }
}