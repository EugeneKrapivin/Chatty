using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chatty.Models;
using Orleans;

namespace Chatty.GrainInterfaces
{
    public interface IServerGrain : IGrainWithStringKey
    {
        #region server management

        Task<string> GetServerNameAsync();

        #endregion
        
        #region room management

        Task<IEnumerable<string>> GetRoomsAsync();

        Task<RoomRegistry> CreateRoomAsync(string username, string roomName, string password, bool isPubliclyVisible, params string[] modorators);

        Task<bool> UpdateRoomRegisteryAsync(UserRegistery user, RoomRegistry roomData);

        Task<bool> DeleteRoomAsync(UserRegistery user, string roomName);

        #endregion

        #region user management

        Task<UserRegistery> RegisterUserAsync(string username);

        Task<bool> RemoveUserAsync(Guid clientId, string username);

        Task<bool> CheckUsernameAsync(string username);

        Task<bool> LogToServerAsync(Guid userToken, string username);
        
        #endregion

    }
}
