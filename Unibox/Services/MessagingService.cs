using NetMessage;
using NetMessage.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Unibox.Helpers;
using Unibox.Messaging.DTOs;
using Unibox.Messaging.Responses;

namespace Unibox.Services
{
    internal class MessagingService
    {
        private NetMessageClient client = new NetMessageClient();
        private LoggingService loggingService;

        public MessagingService(LoggingService loggingService)
        {
            this.loggingService = loggingService;
            client.OnError += Client_OnError;
            client.Connected += OnConnected;
            client.Disconnected += OnDisconnected;
        }

        private void OnDisconnected(NetMessageClient client, SessionClosedArgs args)
        {
            loggingService.WriteLine($"Client disconnected from server. Reason: [{args.Reason}] | Any socket exception: [{args?.SocketException}]");
        }

        private void OnConnected(NetMessageClient client)
        {
            loggingService.WriteLine($"Client connected to server successfully.");
        }

        private void Client_OnError(NetMessageClient arg1, string arg2, Exception? arg3)
        {
            loggingService.WriteLine($"Client error: {arg2} | Exception: {arg3?.Message}");
        }

        private string UncPathToIP(string uncPath)
        {
            string ipAddress = "127.0.0.1";
            Debug.WriteLine(Path.GetPathRoot(uncPath));
            if (Helpers.FileSystem.IsNetworkPath(uncPath))
            {
                string root = Path.GetPathRoot(uncPath); // e.g., "\\ATARI-1280\Users"
                string server = root.TrimStart('\\').Split('\\')[0]; // "ATARI-1280"
                IPHostEntry entry = Dns.GetHostEntry(server);
                foreach (var ip in entry.AddressList)
                {
                    if (Helpers.String.IsValidIPv4Address(ip.ToString()))
                    {
                        ipAddress = ip.ToString();
                        break;
                    }
                }
            }
            return ipAddress;
        }

        internal async Task<AddGameResponse> SendAddGameRequest(string installationPath, GameDTO gameDTO)
        {
            loggingService.WriteLine($"SendAddGame request made of MessageService. InstallationPath: [{installationPath}] | Game: [{gameDTO.Title}]");

            if (string.IsNullOrWhiteSpace(installationPath) || gameDTO is null)
            {
                loggingService.WriteLine("SendAddGameRequest: Invalid parameters provided.");
                return null;
            }

            string ipAddress = UncPathToIP(installationPath);

            loggingService.WriteLine("IP Address discerned from UNC path: " + ipAddress);

            loggingService.WriteLine("Attempting to connect to Unibox plugin server at: " + ipAddress + ":" + Properties.Settings.Default.MessagingPort);

            bool successful = await client.ConnectAsync(ipAddress, Properties.Settings.Default.MessagingPort);

            AddGameResponse response = new AddGameResponse();
            if (!successful)
            {
                response.IsSuccessful = false;
                response.TextResult = "Failed to connect to Unibox plugin server.";
            }
            else
            {
                response = await client.SendRequestAsync(new Messaging.Requests.AddGameRequest { Game = gameDTO });
            }

            client.Disconnect();
            return response;
        }
    }
}