﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
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

namespace Unibox.Views
{
    /// <summary>
    /// Interaction logic for AddInstallationForm.xaml
    /// </summary>
    public partial class AddInstallationForm : Window
    {
        public AddInstallationForm()
        {
            InitializeComponent();
            this.DataContext = App.Current.Services.GetService(typeof(ViewModels.AddInstallationVM));
            this.Owner = Application.Current.MainWindow;
        }

        



    }
}
