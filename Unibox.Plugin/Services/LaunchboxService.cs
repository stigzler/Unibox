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
    }
}