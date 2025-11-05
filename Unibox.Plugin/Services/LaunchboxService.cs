using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Messaging.DTOs;
using Unbroken.LaunchBox.Plugins;

namespace Unibox.Plugin.Services
{
    internal class LaunchboxService
    {
        internal Exception AddGame(GameDTO gameDTO)
        {
            // soz for the try/catch - but LB doesn't return anything to indicate whether adding successful.
            try
            {
                var newGame = PluginHelper.DataManager.AddNewGame(gameDTO.Title);
                newGame.Platform = gameDTO.Platform;
                newGame.Notes = gameDTO.Notes;
                newGame.Publisher = gameDTO.Publisher;
                newGame.Developer = gameDTO.Developer;
                newGame.ApplicationPath = gameDTO.ApplicationPath;
                newGame.EmulatorId = gameDTO.EmulatorID;
                newGame.ReleaseDate = gameDTO.ReleaseDate;
                newGame.DateAdded = gameDTO.DateAdded ?? DateTime.Now;

                PluginHelper.DataManager.Save();

                if (PluginHelper.StateManager.IsBigBox && Properties.Settings.Default.ShowGameOnAdd)
                {
                    PluginHelper.BigBoxMainViewModel.ShowGame(newGame, Unbroken.LaunchBox.Plugins.Data.FilterType.None);
                }
                else if (!PluginHelper.StateManager.IsBigBox)
                {
                    PluginHelper.LaunchBoxMainViewModel.RefreshData();
                }
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }

        internal Exception EditGame(GameDTO gameDTO)
        {
            try
            {
                var existingGame = PluginHelper.DataManager.GetGameById(gameDTO.LaunchboxID);
                if (existingGame == null)
                {
                    return new Exception($"Game with ID {gameDTO.LaunchboxID} not found in LaunchBox database.");
                }
                existingGame.Title = gameDTO.Title;
                existingGame.Notes = gameDTO.Notes;
                existingGame.Publisher = gameDTO.Publisher;
                existingGame.Developer = gameDTO.Developer;
                //existingGame.ApplicationPath = gameDTO.ApplicationPath;
                existingGame.ReleaseDate = gameDTO.ReleaseDate;
                existingGame.DateModified = DateTime.Now;
                PluginHelper.DataManager.Save();
                if (PluginHelper.StateManager.IsBigBox && Properties.Settings.Default.ShowGameOnEdit)
                {
                    PluginHelper.BigBoxMainViewModel.ShowGame(existingGame, Unbroken.LaunchBox.Plugins.Data.FilterType.None);
                }
                else if (!PluginHelper.StateManager.IsBigBox)
                {
                    PluginHelper.LaunchBoxMainViewModel.RefreshData();
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
            return null;
        }

        internal Exception DeleteGame(GameDTO gameDTO)
        {
            try
            {
                var existingGame = PluginHelper.DataManager.GetGameById(gameDTO.LaunchboxID);
                if (existingGame == null)
                {
                    return new Exception($"Game with ID {gameDTO.LaunchboxID} not found in LaunchBox database.");
                }
                bool successful = PluginHelper.DataManager.TryRemoveGame(existingGame);

                if (!successful)
                {
                    return new Exception($"Launchbox returned it could not delete game with ID {gameDTO}.");
                }

                PluginHelper.DataManager.Save();
                if (!PluginHelper.StateManager.IsBigBox)
                {
                    PluginHelper.LaunchBoxMainViewModel.RefreshData();
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
            return null;
        }
    }
}