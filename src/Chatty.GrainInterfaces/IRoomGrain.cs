using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chatty.Models;
using Orleans;

namespace Chatty.GrainInterfaces
{
    public interface IRoomGrain : IGrainWithStringKey
    {
        Task<string> GetRoomName();
        Task<bool> ClientEnterAsync(string clientId);
        Task MessageAsync(string message);
        Task SetupRoomAsync(RoomRegistry createdRoom);
    }
}
