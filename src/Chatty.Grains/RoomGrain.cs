using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chatty.GrainInterfaces;
using Chatty.Models;
using Orleans;

namespace Chatty.Grains
{
    public class RoomGrain : Grain, IRoomGrain
    {
        private string _roomName;

        private List<string> _modorators = new List<string>();
        private string _owner;
        private List<string> _messages = new List<string>();
        private List<string> _user = new List<string>();
        public Task<string> GetRoomName() 
            => Task.FromResult(_roomName);

        public Task<bool> ClientEnterAsync(string clientId)
        {
            if (_user.Contains(clientId))
            {
                throw new ArgumentException("User can not re-enter a room");
            }

            _user.Add(clientId);

            return Task.FromResult(true);
        }

        public Task MessageAsync(UserRegistery user, string message)
        {
            
        }

        public Task SetupRoomAsync(RoomRegistry createdRoom)
        {
            throw new NotImplementedException();
        }
    }
}
