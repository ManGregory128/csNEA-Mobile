using System;
using System.Collections.Generic;
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

        public Settings()
        {

        }
    }
}
