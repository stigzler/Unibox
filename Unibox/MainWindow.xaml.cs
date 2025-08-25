﻿using System.Windows;
using System.Windows.Media;

namespace Unibox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = App.Current.Services.GetService(typeof(ViewModels.MainWindowVM));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.AppMainHeight = this.Height;
            Properties.Settings.Default.AppMainWidth = this.Width;
            Properties.Settings.Default.Save();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Properties.Settings.Default.AppMainHeight = this.Height;
            Properties.Settings.Default.AppMainWidth = this.Width;
            Properties.Settings.Default.Save();
        }
    }
}