using AdonisUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Unibox.Plugin.Helpers
{
    public static class Theming
    {
        public static void ApplyTheme()
        {
            if (Properties.Settings.Default.AppDarkMode)
            {
                // Change to Dark Theme
                AdonisUI.ResourceLocator.SetColorScheme(Application.Current.Resources, ResourceLocator.DarkColorScheme);
            }
            else
            {
                // Change to Light Theme
                AdonisUI.ResourceLocator.SetColorScheme(Application.Current.Resources, ResourceLocator.LightColorScheme);
            }
        }
    }
}