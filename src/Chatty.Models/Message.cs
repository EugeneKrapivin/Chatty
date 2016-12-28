using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatty.Models
{
    public class Message
    {
        public string Contect { get; set; }
        public DateTime SentAt { get; set; }
        public string SentBy { get; set; }
    }
}
