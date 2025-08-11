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
    /// Interaction logic for AddGameResultsWindow.xaml
    /// </summary>
    public partial class AddGameResultsWindow : Window
    {
        public AddGameResultsVM ViewModel => this.DataContext as AddGameResultsVM;

        public AddGameResultsWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.DataContext = App.Current.Services.GetService(typeof(ViewModels.AddGameResultsVM));
            Helpers.Theming.ApplyTheme();
        }

        private void CloseBT_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}