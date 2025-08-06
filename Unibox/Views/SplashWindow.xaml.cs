using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Unibox.Messages;
using Unibox.Services;
using Unibox.ViewModels;

namespace Unibox.Views
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();

            Helpers.Theming.ApplyTheme();

            this.DataContext = App.Current.Services.GetService(typeof(ViewModels.SplashVM));

            WeakReferenceMessenger.Default.Register<CloseSplashMessage>(this, (r, m) => this.Close());
        }

        // Messy approach but meh
        protected async override void OnContentRendered(EventArgs e)
        {
            DatabaseService databaseService = (DatabaseService)App.Current.Services.GetService(typeof(DatabaseService));
            await Task.Run(() => databaseService.EnsureTextBasedDataPopulated());
            this.Close();
        }
    }
}