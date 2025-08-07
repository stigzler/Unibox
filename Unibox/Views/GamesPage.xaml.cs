using System.Windows.Controls;

namespace Unibox.Views
{
    /// <summary>
    /// Interaction logic for GamesPage.xaml
    /// </summary>
    public partial class GamesPage : UserControl
    {
        public GamesPage()
        {
            InitializeComponent();
            this.DataContext = App.Current.Services.GetService(typeof(ViewModels.GamesVM));
        }
    }
}