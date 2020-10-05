using Microsoft.AspNetCore.Http;
using QuestRoom.Data;
using QuestRoom.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuestRoom.ViewModel
{
    public class RoomViewModel : Room
    {
        [Required]
        [Display(Name = "File")]
        public IFormFileCollection Files { get; set; }
    }
}
