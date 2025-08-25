using System;
using System.Collections.Generic;
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
using Unibox.ViewModels;

namespace Unibox.Views
{
    /// <summary>
    /// Interaction logic for PleaseWaitWindow.xaml
    /// </summary>
    public partial class PleaseWaitWindow : Window
    {
        private bool closedByApplication = false;
        internal PleaseWaitVM ViewModel => this.DataContext as PleaseWaitVM;

        public PleaseWaitWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            Application.Current.MainWindow.IsEnabled = false;
            this.DataContext = App.Current.Services.GetService(typeof(ViewModels.PleaseWaitVM));
            Helpers.Theming.ApplyTheme();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!closedByApplication) e.Cancel = true;
        }

        public void CloseWindow()
        {
            closedByApplication = true;
            Application.Current.MainWindow.IsEnabled = true;
            this.Close();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Properties.Settings.Default.AppRomUploadHeight = this.Height;
            Properties.Settings.Default.AppRomUploadWidth = this.Width;
            Properties.Settings.Default.Save();
        }
    }
}