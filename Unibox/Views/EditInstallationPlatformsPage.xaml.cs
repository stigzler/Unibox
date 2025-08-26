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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unibox.ViewModels;

namespace Unibox.Views
{
    /// <summary>
    /// Interaction logic for EditInstallationPlatformsPage.xaml
    /// </summary>
    public partial class EditInstallationPlatformsPage : UserControl
    {
        public EditInstallationPlatformsVM ViewModel => (EditInstallationPlatformsVM)this.DataContext;

        public EditInstallationPlatformsPage()
        {
            InitializeComponent();
            this.DataContext = App.Current.Services.GetService(typeof(ViewModels.EditInstallationPlatformsVM));
        }
    }
}