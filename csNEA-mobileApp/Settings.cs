using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Xamarin.Essentials;

namespace csNEA_mobileApp
{
    class Settings
    {        
        public static bool FirstRun
        {
            get => Preferences.Get(nameof(FirstRun), true);
            set => Preferences.Set(nameof(FirstRun), value);
        }      
        
        public static string CurrentUsername
        {
            get => Preferences.Get(nameof(CurrentUsername), null);
            set => Preferences.Set(nameof(CurrentUsername), value);
        }
        public static string CurrentPassword
        {
            get => Preferences.Get(nameof(CurrentPassword), null);
            set => Preferences.Set(nameof(CurrentPassword), value);
        }

        public static string CurrentDatabase
        {
            get => Preferences.Get(nameof(CurrentDatabase), null);
            set => Preferences.Set(nameof(CurrentDatabase), value);
        }

        public Settings()
        {

        }
    }
}
