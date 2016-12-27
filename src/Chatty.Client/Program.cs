using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Chatty.GrainInterfaces;
using Chatty.Models;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;

namespace Chatty.Client
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var config = ClientConfiguration.LocalhostSilo();
            try
            {
                InitializeWithRetries(config, initializeAttemptsBeforeFailing: 5);
                var machineKey = Guid.NewGuid();
                new Program().MainAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Orleans client initialization failed failed due to {ex}");

                Console.ReadLine();
                return 1;
            }

            Console.WriteLine("Press Enter to terminate...");
            Console.ReadLine();
            return 0;
        }

        private static void InitializeWithRetries(ClientConfiguration config, int initializeAttemptsBeforeFailing)
        {
            var attempt = 0;
            while (true)
            {
                try
                {
                    GrainClient.Initialize(config);
                    Console.WriteLine("Client successfully connect to silo host");
                    break;
                }
                catch (SiloUnavailableException)
                {
                    attempt++;
                    Console.WriteLine($"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
                    if (attempt > initializeAttemptsBeforeFailing)
                    {
                        throw;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }
        }

        private async Task MainAsync(string serverName = "local")
        {
            var username = "nocgod";

            var server = GrainClient.GrainFactory.GetGrain<IServerGrain>(serverName);
            Console.WriteLine($"Registering with server {serverName} using {username} as username...");
            UserRegistery user;
            do
            {
                Console.Write("Please enter your wanted username: ");
                username = Console.ReadLine();
                user = await server.RegisterUserAsync(username);
                if (user == null)
                {
                    Console.WriteLine("Username already taken");
                }
            } while (user == null);


            Console.WriteLine($"Logging in to server...");
            if (await server.LogToServerAsync(user.UserToken, user.Username))
            {
                var rooms = await server.GetRoomsAsync();
                foreach (var room in rooms)
                {
                    Console.WriteLine(room);
                }
            }
        }
    }
}
