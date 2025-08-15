using NetMessage;
using NetMessage.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public MessagingService()
        {
            client.OnError += Client_OnError;
            client.Connected += OnConnected;
            client.Disconnected += OnDisconnected;
        }

        private void OnDisconnected(NetMessageClient client, SessionClosedArgs args)
        {
            Log.WriteLine($"Client disconnected from server. Reason: [{args.Reason}] | Any socket exception: [{args?.SocketException}]");
        }

        private void OnConnected(NetMessageClient client)
        {
            Log.WriteLine($"Client connected to server successfully.");
        }

        private void Client_OnError(NetMessageClient arg1, string arg2, Exception? arg3)
        {
            Log.WriteLine($"Client error: {arg2} | Exception: {arg3?.Message}");
        }

        private string UncPathToIP(string uncPath)
        {
            string ipAddress = "127.0.0.1";
            if (Helpers.FileSystem.IsNetworkPath(uncPath))
            {
                IPHostEntry entry = Dns.GetHostEntry(uncPath);
                foreach (var ip in entry.AddressList)
                {
                    if (Helpers.String.IsValidIPv4Address(ip.ToString()))
                    {
                        ipAddress = ip.ToString();
                        break;
                    }
                }
            }
            return ipAddress; // Return original path if no valid IP found
        }

        internal async Task<AddGameResponse> SendAddGameRequest(string installationPath, GameDTO gameDTO)
        {
            Log.WriteLine($"SendAddGame request made of MessageService. InstallationPath: [{installationPath}] | Game: [{gameDTO.Title}]");

            if (string.IsNullOrWhiteSpace(installationPath) || gameDTO is null)
            {
                Log.WriteLine("SendAddGameRequest: Invalid parameters provided.");
                return null;
            }

            string ipAddress = UncPathToIP(installationPath);

            Log.WriteLine("IP Address discerned from UNC path: " + ipAddress);

            Log.WriteLine("Attempting to connect to Unibox plugin server at: " + ipAddress + ":" + Properties.Settings.Default.MessagingPort);

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