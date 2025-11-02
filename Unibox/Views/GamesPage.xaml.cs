using System.Windows.Controls;
using Unibox.ViewModels;

namespace Unibox.Views
{
    /// <summary>
    /// Interaction logic for GamesPage.xaml
    /// </summary>
    public partial class GamesPage : UserControl
    {
        internal GamesVM ViewModel => (GamesVM)this.DataContext;

        public GamesPage()
        {
            InitializeComponent();
            this.DataContext = App.Current.Services.GetService(typeof(ViewModels.GamesVM));

            PlatformNotesPN.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ToggleNotesBT_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PlatformNotesPN.Visibility == System.Windows.Visibility.Visible)
                PlatformNotesPN.Visibility = System.Windows.Visibility.Collapsed;
            else
                PlatformNotesPN.Visibility = System.Windows.Visibility.Visible;
        }
    }
}