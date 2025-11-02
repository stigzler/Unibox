using NetMessage;
using NetMessage.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Unibox.Messaging.Requests;
using Unibox.Messaging.Responses;
using Unibox.Plugin.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Unibox.Plugin.Services
{
    internal class MessagingService
    {
        private LaunchboxService launchboxService;
        private LoggingService loggingService;

        public MessagingService(LaunchboxService launchboxService, LoggingService loggingService)
        {
            this.launchboxService = launchboxService;
            this.loggingService = loggingService;

            loggingService.WriteLine("Starting MessagingService on port: " + Properties.Settings.Default.Port);

            var server = new NetMessageServer(Properties.Settings.Default.Port);
            server.OnError += OnError;
            server.SessionOpened += OnSessionOpened;
            server.SessionClosed += OnSessionClosed;
            server.AddRequestHandler<AddGameRequest, AddGameResponse>(AddGameRequestHandler);
            server.AddRequestHandler<EditGameRequest, EditGameResponse>(EditGameRequestHandler);

            server.Start();

            loggingService.WriteLine("MessagingService started successfully.");
        }

        private void EditGameRequestHandler(NetMessageSession session, TypedRequest<EditGameRequest, EditGameResponse> request)
        {
            loggingService.WriteLine($"Received EditGameRequest for game: {request.Request.Game.Title} ({request.Request.Game.Platform})");

            loggingService.WriteLine("Attempting Game Edit");

            Exception anyException = launchboxService.EditGame(request.Request.Game);

            var response = new EditGameResponse();

            if (anyException != null)
            {
                loggingService.WriteLine($"Error editing game: {anyException.Message}");
                response.TextResult = $"Error whilst editing game in Launchbox Database: '{anyException.Message}'";
            }
            else
            {
                loggingService.WriteLine($"Game [{request.Request.Game.Title}] edited successfully!");
                response.TextResult = $"Game [{request.Request.Game.Title}] edited successfully in Launchbox Database.";
                response.IsSuccessful = true;
            }

            request.SendResponseAsync(response);
        }

        private void AddGameRequestHandler(NetMessageSession session, TypedRequest<AddGameRequest, AddGameResponse> request)
        {
            loggingService.WriteLine($"Received AddGameRequest for game: {request.Request.Game.Title} ({request.Request.Game.Platform})");

            loggingService.WriteLine("Attempting add to Launchbox Database");

            Exception anyException = launchboxService.AddGame(request.Request.Game);

            var response = new AddGameResponse();

            if (anyException != null)
            {
                loggingService.WriteLine($"Error adding game: {anyException.Message}");
                response.TextResult = $"Error whilst adding game to Launchbox Database: '{anyException.Message}'";
            }
            else
            {
                loggingService.WriteLine($"Game [{request.Request.Game.Title}] added successfully!");
                response.TextResult = $"Game [{request.Request.Game.Title}] added successfully to Launchbox Database.";
                response.IsSuccessful = true;
            }

            request.SendResponseAsync(response);
        }

        private void OnSessionClosed(NetMessageSession session, SessionClosedArgs args)
        {
            loggingService.WriteLine($"Session closed. Session ID: [{session.Guid}] Reason: {args.Reason}");
        }

        private void OnSessionOpened(NetMessageSession session)
        {
            loggingService.WriteLine($"Session opened. Session ID: [{session.Guid}].");
        }

        private void OnError(NetMessageServer server, NetMessageSession? session, string arg3, Exception? exception)
        {
            loggingService.WriteLine($"Error occurred in MessagingService. Session ID: [{session?.Guid}]. Error: {arg3}. Exception: {exception?.Message}");
        }
    }
}