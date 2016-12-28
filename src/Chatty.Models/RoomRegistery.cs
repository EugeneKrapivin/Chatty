using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatty.Models
{
    public class RoomRegistry
    {
        public string RoomName { get; set; }
        public Guid RoomKey { get; set; }
        public string RoomPassword { get; set; }
        public bool IsPubliclyVisible { get; set; }
        public List<string> Modorators { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
