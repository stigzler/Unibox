using Microsoft.Extensions.DependencyInjection;
using NetMessage;
using NetMessage.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Messaging.Requests;
using Unibox.Messaging.Responses;
using Unibox.Plugin.Data.Messages;
using Unibox.Plugin.Helpers;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Unibox.Plugin.Services
{
    internal class MessageService
    {
        private LaunchboxService launchboxService;

        public MessageService(LaunchboxService launchboxService)
        {
            this.launchboxService = launchboxService;

            var server = new NetMessageServer(1234);

            server.OnError += OnError;
            server.SessionOpened += OnSessionOpened;
            server.SessionClosed += OnSessionClosed;

            server.AddMessageHandler<string>(StringMessageHandler);
            server.AddRequestHandler<AddGameRequest, AddGameResponse>(AddGameRequestHandler);

            server.Start();
        }

        private void AddGameRequestHandler(NetMessageSession session, TypedRequest<AddGameRequest, AddGameResponse> request)
        {
            var response = new AddGameResponse
            {
                Result = $"Game '{request.Request.Game.Title}' added successfully!",
            };
            request.SendResponseAsync(response);
        }

        private void StringMessageHandler(NetMessageSession session, string arg2)
        {
            Log.WriteLine($"Received string message: {arg2}");
        }

        private void OnSessionClosed(NetMessageSession session, SessionClosedArgs args)
        {
            Log.WriteLine($"Seesion Closed: {args.Reason.ToString()}");
        }

        private void OnSessionOpened(NetMessageSession session)
        {
            Log.WriteLine($"Session Opened: {session.ToString()}");
        }

        private void OnError(NetMessageServer server, NetMessageSession? session, string arg3, Exception? exception)
        {
            Log.WriteLine($"Error: {arg3}: {exception?.ToString()}");
        }
    }
}