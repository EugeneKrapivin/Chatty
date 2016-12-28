using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chatty.GrainInterfaces;
using Chatty.Models;
using Orleans;

namespace Chatty.Grains
{
    public class ServerGrain : Grain, IServerGrain
    {
        private string _serverName;
        public List<RoomRegistry> Rooms { get; set; } = new List<RoomRegistry>();
        public List<UserRegistery> Users { get; set; } = new List<UserRegistery>();

        public override Task OnActivateAsync()
        {
            _serverName = this.GetPrimaryKeyString();
            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            // TODO: all online users should be gracefully disconnected
            return base.OnDeactivateAsync();
        }

        public Task<string> GetServerNameAsync() => Task.FromResult(_serverName);

        public Task<IEnumerable<string>> GetRoomsAsync() => Task.FromResult(Rooms.Select(r => r.RoomName));

        public Task<UserRegistery> RegisterUserAsync(string username)
        {
            if (Users.Any(user => user.Username.Equals(username, StringComparison.CurrentCulture)))
            {
                return Task.FromResult((UserRegistery) null);
            }
            var currentUser = new UserRegistery
            {
                Username = username,
                UserToken = Guid.NewGuid(),
                LastConnectionDate = DateTime.UtcNow
            };
            Users.Add(currentUser);

            return Task.FromResult(currentUser);
            
        }

        public Task<bool> RemoveUserAsync(Guid userToken, string username)
        {
            var currentUser = Users.SingleOrDefault(user => user.UserToken.Equals(userToken) && user.Username.Equals(username, StringComparison.Ordinal));
            if (currentUser != null)
            {
                Users.Remove(currentUser);
            }
            else
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> CheckUsernameAsync(string username) 
            => Task.FromResult(Users.Any(user => user.Username.Equals(username, StringComparison.Ordinal)));

        public Task<bool> LogToServerAsync(Guid userToken, string username)
        {
            var currentUser = Users.SingleOrDefault(user => user.UserToken.Equals(userToken) && user.Username.Equals(username));
            if (currentUser != null)
            {
                currentUser.LastConnectionDate = DateTime.UtcNow;
            }
            else
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public async Task<RoomRegistry> CreateRoomAsync(string username, string roomName, string password, bool isPubliclyVisible, params string[] modorators)
        {
            if (Rooms.Any(room => room.RoomName.Equals(roomName, StringComparison.Ordinal)))
            {
                return null;
            }

            var createdRoom = new RoomRegistry
            {
                RoomName = roomName,
                RoomKey = Guid.NewGuid(),
                RoomPassword = password,
                IsPubliclyVisible = isPubliclyVisible,
                Modorators = new [] { username }.ToList(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = username
            };

            Rooms.Add(createdRoom);
            await GrainFactory.GetGrain<IRoomGrain>(roomName).SetupRoomAsync(createdRoom);

            return createdRoom;
        }

        public async Task<bool> UpdateRoomRegisteryAsync(UserRegistery user, RoomRegistry roomData)
        {
            if (!Users.Any(r => r.Username.Equals(user.Username, StringComparison.Ordinal) 
                    && r.UserToken == user.UserToken))
            {
                return false;
            }

            if (!roomData.CreatedBy.Equals(user.Username))
            {
                return false;
            }

            await GrainFactory.GetGrain<IRoomGrain>(roomData.RoomName).SetupRoomAsync(roomData);

            return true;
        }

        public Task<bool> DeleteRoomAsync(UserRegistery user, string roomName)
        {
            if (!Users.Any(r => r.Username.Equals(user.Username, StringComparison.Ordinal)
                    && r.UserToken == user.UserToken))
            {
                return Task.FromResult(false);
            }

            var room = Rooms.SingleOrDefault(r => r.RoomName.Equals(roomName, StringComparison.Ordinal));

            if (room == null)
            {
                return Task.FromResult(false);
            }

            room.IsDeleted = true;
            room.IsPubliclyVisible = false;

            return Task.FromResult(true);
        }
    }
}
