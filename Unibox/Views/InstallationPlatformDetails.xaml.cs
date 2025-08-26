using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Unibox.Data.Models;
using Unibox.ViewModels;

namespace Unibox.Views
{
    /// <summary>
    /// Interaction logic for InstallationPlatformDetails.xaml
    /// </summary>
    public partial class InstallationPlatformDetails : Window
    {
        public EditInstallationPlatformsVM ViewModel => this.DataContext as EditInstallationPlatformsVM;

        public InstallationPlatformDetails()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = App.Current.Services.GetService(typeof(ViewModels.EditInstallationPlatformsVM));
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Properties.Settings.Default.AppPlatformDetailsHeight = this.Height;
            Properties.Settings.Default.AppPlatformDetailsWidth = this.Width;
            Properties.Settings.Default.Save();
        }
    }
}