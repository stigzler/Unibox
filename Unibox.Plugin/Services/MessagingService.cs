using NetMessage;
using NetMessage.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Schema;
using Unbroken.LaunchBox.Plugins.Data;
using Unibox.Messaging.DTOs;
using Unibox.Messaging.Messages;
using Unibox.Messaging.Requests;
using Unibox.Messaging.Responses;
using Unibox.Plugin.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Unibox.Plugin.Services
{
    internal class MessagingService
    {
        private LaunchboxService _launchboxService;
        private LoggingService _loggingService;

        private NetMessageServer server = new NetMessageServer(Properties.Settings.Default.Port);

        // keep track of active sessions so the server can push messages to connected clients
        private readonly ConcurrentDictionary<Guid, NetMessageSession> _sessions = new();

        public MessagingService(LaunchboxService launchboxService, LoggingService loggingService)
        {
            this._launchboxService = launchboxService;
            this._loggingService = loggingService;

            loggingService.WriteLine("Starting MessagingService on port: " + Properties.Settings.Default.Port);

            server.OnError += OnError;
            server.SessionOpened += OnSessionOpened;
            server.SessionClosed += OnSessionClosed;
            server.AddRequestHandler<AddGameRequest, AddGameResponse>(AddGameRequestHandler);
            server.AddRequestHandler<EditGameRequest, EditGameResponse>(EditGameRequestHandler);
            server.AddRequestHandler<DeleteGameRequest, DeleteGameResponse>(DeleteGameRequestHandler);
            server.AddRequestHandler<GetCurrentGameRequest, GetCurrentGameResponse>(GetCurrentGameRequestHandler);

            server.Start();

            loggingService.WriteLine("MessagingService started successfully.");
        }

        private void GetCurrentGameRequestHandler(NetMessageSession session, TypedRequest<GetCurrentGameRequest, GetCurrentGameResponse> request)
        {
            //_loggingService.WriteLine($"Received GetCurrentGameRequest");

            IGame game = _launchboxService.GetCurrentGame();

            var response = new GetCurrentGameResponse
            {
                Game = new GameDTO(game)
            };

            request.SendResponseAsync(response);
        }

        private void EditGameRequestHandler(NetMessageSession session, TypedRequest<EditGameRequest, EditGameResponse> request)
        {
            _loggingService.WriteLine($"Received EditGameRequest for game: {request.Request.Game.Title} ({request.Request.Game.Platform})");

            _loggingService.WriteLine("Attempting Game Edit");

            Exception anyException = _launchboxService.EditGame(request.Request.Game);

            var response = new EditGameResponse();

            if (anyException != null)
            {
                _loggingService.WriteLine($"Error editing game: {anyException.Message}");
                response.TextResult = $"Error whilst editing game in Launchbox Database: '{anyException.Message}'";
            }
            else
            {
                _loggingService.WriteLine($"Game [{request.Request.Game.Title}] edited successfully!");
                response.TextResult = $"Game [{request.Request.Game.Title}] edited successfully in Launchbox Database.";
                response.IsSuccessful = true;
            }

            request.SendResponseAsync(response);
        }

        private void DeleteGameRequestHandler(NetMessageSession session, TypedRequest<DeleteGameRequest, DeleteGameResponse> request)
        {
            _loggingService.WriteLine($"Received DeleteGameRequest for game: {request.Request.Game.Title} ({request.Request.Game.Platform})");
            _loggingService.WriteLine("Attempting Game Deletion");
            Exception anyException = _launchboxService.DeleteGame(request.Request.Game);
            var response = new DeleteGameResponse();
            if (anyException != null)
            {
                _loggingService.WriteLine($"Error deleting game: {anyException.Message}");
                response.TextResult = $"Exception: {anyException.Message}";
            }
            else
            {
                _loggingService.WriteLine($"Game [{request.Request.Game.Title}] deleted successfully!");
                response.TextResult = $"Game [{request.Request.Game.Title}] deleted successfully from Launchbox Database.";
                response.IsSuccessful = true;
            }
            request.SendResponseAsync(response);
        }

        private void AddGameRequestHandler(NetMessageSession session, TypedRequest<AddGameRequest, AddGameResponse> request)
        {
            _loggingService.WriteLine($"Received AddGameRequest for game: {request.Request.Game.Title} ({request.Request.Game.Platform})");

            _loggingService.WriteLine("Attempting add to Launchbox Database");

            Exception anyException = _launchboxService.AddGame(request.Request.Game);

            var response = new AddGameResponse();

            if (anyException != null)
            {
                _loggingService.WriteLine($"Error adding game: {anyException.Message}");
                response.TextResult = $"Error whilst adding game to Launchbox Database: '{anyException.Message}'";
            }
            else
            {
                _loggingService.WriteLine($"Game [{request.Request.Game.Title}] added successfully!");
                response.TextResult = $"Game [{request.Request.Game.Title}] added successfully to Launchbox Database.";
                response.IsSuccessful = true;
            }

            request.SendResponseAsync(response);
        }

        internal async void SendGameChangedMessage(IGame game)
        {
            if (game == null)
            {
                _loggingService.WriteLine("SendGameChangedMessage called with null game.");
                return;
            }

            var message = new GameChangedMessage(game);

            // make a snapshot to avoid collection modified exceptions
            var sessions = _sessions.Values.ToArray();

            foreach (var session in sessions)
            {
                try
                {
                    // Most recent NetMessage versions expose SendMessageAsync on the session.
                    // If your version exposes a server.SendMessageAsync(session, message) method, use that instead.
                    await session.SendMessageAsync(message);
                    _loggingService.WriteLine($"Sent GameChangedMessage to session {session.Guid}.");
                }
                catch (MissingMethodException)
                {
                    // fallback comment: if compilation fails here, call server.SendMessageAsync(session, message) instead.
                    _loggingService.WriteLine($"Failed to send GameChangedMessage to session {session.Guid} - method not available.");
                }
                catch (Exception ex)
                {
                    _loggingService.WriteLine($"Failed to send GameChangedMessage to session {session.Guid}: {ex.Message}");
                }
            }
        }

        private void OnSessionClosed(NetMessageSession session, SessionClosedArgs args)
        {
            _loggingService.WriteLine($"Session closed. Session ID: [{session.Guid}] Reason: {args.Reason}");
            _sessions.TryRemove(session.Guid, out _);
        }

        private void OnSessionOpened(NetMessageSession session)
        {
            _loggingService.WriteLine($"Session opened. Session ID: [{session.Guid}].");
            _sessions.TryAdd(session.Guid, session);
        }

        private void OnError(NetMessageServer server, NetMessageSession? session, string arg3, Exception? exception)
        {
            _loggingService.WriteLine($"Error occurred in MessagingService. Session ID: [{session?.Guid}]. Error: {arg3}. Exception: {exception?.Message}");
        }
    }
}