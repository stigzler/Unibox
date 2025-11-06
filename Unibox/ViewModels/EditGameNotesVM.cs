using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unibox.Data.Models;
using Unibox.Messages;

namespace Unibox.ViewModels
{
    internal partial class EditGameNotesVM : ObservableObject
    {
        [ObservableProperty]
        private GameModel game;

        [ObservableProperty]
        private string notesText;

        internal void SetGame(GameModel game)
        {
            Game = game;
            NotesText = Game.Notes;
        }

        [RelayCommand]
        private void SaveNotes()
        {
            Game.Notes = NotesText;
            GoBack();
        }

        [RelayCommand]
        private void Paste()
        {
            NotesText = System.Windows.Clipboard.GetText();
        }

        [RelayCommand]
        private void GoBack()
        {
            WeakReferenceMessenger.Default.Send(new PageChangeMessage(new PageChangeMessageArgs()
            {
                RequestType = Data.Enums.PageRequestType.EditGame,
                Data = new Messages.MessageDetails.EditGameMessageDetails()
                {
                    DoNotChangeGameAndInstallation = true,
                    RefreshGame = true
                }
            }));
        }
    }
}