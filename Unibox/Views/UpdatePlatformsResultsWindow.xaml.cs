using Microsoft.Extensions.DependencyInjection;
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
    /// Interaction logic for UpdatePlatformsResultsWindow.xaml
    /// </summary>
    public partial class UpdatePlatformsResultsWindow : Window
    {
        public UpdatePlatformsResultsVM ViewModel => (UpdatePlatformsResultsVM)this.DataContext;

        public UpdatePlatformsResultsWindow()
        {
            InitializeComponent();
            this.DataContext = App.Current.Services.GetService<UpdatePlatformsResultsVM>();
            this.Owner = Application.Current.MainWindow;
        }

        private void CloseBT_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}