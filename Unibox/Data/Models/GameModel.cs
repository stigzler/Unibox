using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unbroken.LaunchBox.Plugins.Data;
using Unibox.Messaging.DTOs;

namespace Unibox.Data.Models
{
    public class GameModel
    {
        public ObjectId ID { get; set; } = ObjectId.NewObjectId();
        public string Title { get; set; } = string.Empty;
        public PlatformModel Platform { get; set; } = null;
        public string ApplicationPath { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; } = DateTime.MinValue;
        public string Notes { get; set; } = string.Empty;
        public string LaunchboxID { get; set; } = string.Empty;

        public GameModel()
        {
        }

        public GameModel(IGame game)
        {
            Title = game.Title;
            // Platform = game.Platform; // urgh
            ApplicationPath = game.ApplicationPath;
            Developer = game.Developer;
            Publisher = game.Publisher;
            ReleaseDate = (DateTime)game.ReleaseDate;
            Notes = game.Notes;
            LaunchboxID = game.Id;
        }

        public GameModel(GameDTO game)
        {
            Title = game.Title;
            // Platform = game.Platform; // urgh
            ApplicationPath = game.ApplicationPath;
            Developer = game.Developer;
            Publisher = game.Publisher;
            ReleaseDate = game.ReleaseDate ?? DateTime.MinValue;
            Notes = game.Notes;
            LaunchboxID = game.LaunchboxID;
        }
    }
}