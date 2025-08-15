using NetMessage;
using NetMessage.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Messaging.Requests;
using Unibox.Messaging.Responses;
using Unibox.Plugin.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Unibox.Plugin.Services
{
    internal class MessagingService
    {
        private LaunchboxService launchboxService;

        public MessagingService(LaunchboxService launchboxService)
        {
            this.launchboxService = launchboxService;

            Log.WriteLine("Starting MessagingService on port: " + Properties.Settings.Default.Port);

            var server = new NetMessageServer(Unibox.Plugin.Properties.Settings.Default.Port);
            server.OnError += OnError;
            server.SessionOpened += OnSessionOpened;
            server.SessionClosed += OnSessionClosed;
            server.AddRequestHandler<AddGameRequest, AddGameResponse>(AddGameRequestHandler);

            server.Start();

            Log.WriteLine("MessagingService started successfully.");
        }

        private void AddGameRequestHandler(NetMessageSession session, TypedRequest<AddGameRequest, AddGameResponse> request)
        {
            Log.WriteLine($"Received AddGameRequest for game: {request.Request.Game.Title}");

            Log.WriteLine("Attempting add to Launchbox Database");

            Exception anyException = launchboxService.AddGame(request.Request.Game);

            var response = new AddGameResponse();

            if (anyException != null)
            {
                Log.WriteLine($"Error adding game: {anyException.Message}");
                response.TextResult = $"Error whilst adding game to Launchbox Database: '{anyException.Message}'";
            }
            else
            {
                Log.WriteLine($"Game [{request.Request.Game.Title}] added successfully!");
                response.TextResult = $"Game [{request.Request.Game.Title}] added successfully to Launchbox Database.";
                response.IsSuccessful = true;
            }

            request.SendResponseAsync(response);
        }

        private void OnSessionClosed(NetMessageSession session, SessionClosedArgs args)
        {
            Log.WriteLine($"Session closed. Session ID: [{session.Guid}] Reason: {args.Reason}");
        }

        private void OnSessionOpened(NetMessageSession session)
        {
            Log.WriteLine($"Session opened. Session ID: [{session.Guid}].");
        }

        private void OnError(NetMessageServer server, NetMessageSession? session, string arg3, Exception? exception)
        {
            Log.WriteLine($"Error occurred in MessagingService. Session ID: [{session?.Guid}]. Error: {arg3}. Exception: {exception?.Message}");
        }
    }
}