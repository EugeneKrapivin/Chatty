using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatty.Models
{
    public class UserRegistery
    {
        public string Username { get; set; }
        public DateTime LastConnectionDate { get; set; }
        public Guid UserToken { get; set; }
        public List<string> FriendList { get; set; }
    }
}
