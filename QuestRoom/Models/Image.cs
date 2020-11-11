using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestRoom.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string Path { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
