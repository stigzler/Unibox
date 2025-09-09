using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Unibox.Views
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            InitializeComponent();
            this.DataContext = App.Current.Services.GetService(typeof(ViewModels.SettingsVM));
        }

        void s_PreviewMouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem && e.LeftButton == MouseButtonState.Pressed)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                draggedItem.IsSelected = true;
            }
        }

        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void PortNumberTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void FontSizeTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsTextAllowed(e.Text)) e.Handled = true;
        }

        private void ButtonPaddingTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void FontSizeTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!IsTextAllowed(FontSizeTB.Text) || Convert.ToInt16(FontSizeTB.Text) < 10 || Convert.ToInt16(FontSizeTB.Text) > 30)
            {
                AdonisUI.Controls.MessageBox.Show($"Invalid number or size. Font Size must be between 10 and 30. Reset to 14", "Invalid Font Size",
                    AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Warning);
                Properties.Settings.Default.AppFontSize = 14;
            }
        }

        private void ButtonPaddingTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!IsTextAllowed(ButtonPaddingTB.Text) || Convert.ToInt16(ButtonPaddingTB.Text) < 2 || Convert.ToInt16(ButtonPaddingTB.Text) > 20)
            {
                AdonisUI.Controls.MessageBox.Show($"Invalid number or size. Button Padding must be between 2 and 20. Reset to 5", "Invalid Button Padding",
                    AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Warning);
                Properties.Settings.Default.AppButtonPadding = 5;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ListPaddingTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void ListPaddingTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!IsTextAllowed(ListPaddingTB.Text) || Convert.ToInt16(ListPaddingTB.Text) < 2 || Convert.ToInt16(ListPaddingTB.Text) > 20)
            {
                AdonisUI.Controls.MessageBox.Show($"Invalid number or size. Button Padding must be between 2 and 20. Reset to 5", "Invalid Button Padding",
                    AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Warning);
                Properties.Settings.Default.AppListPadding = 5;
            }
        }
    }
}