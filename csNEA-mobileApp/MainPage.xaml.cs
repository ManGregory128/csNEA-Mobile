using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace csNEA_mobileApp
{
    public partial class MainPage : TabbedPage
    {       
        public MainPage()
        {           
            if (Settings.FirstRun == true)
            {
                LoginScreen modalPage = new LoginScreen();
                this.Navigation.PushModalAsync(modalPage);
            }
            InitializeComponent();
            frameMsg.Text = "Good morning, " + Settings.CurrentUsername + "!";
            
            //DateTime localDate = DateTime.Now;
            //BindingContext = new InitializationClass();
        }

        private void BtnChangePasswd_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnLogOut_Clicked(object sender, EventArgs e)
        {
            LogOut();
        }
        async void LogOut()
        {
            bool answer = await DisplayAlert("CONFIRM", "Are you sure you want to log out?", "Yes", "No");
            if (answer == true)
            {
                await DisplayAlert("Alert", "You have logged out. Restart of application is recommended.", "OK");
                Settings.FirstRun = true;
                LoginScreen modalPage = new LoginScreen();
                await this.Navigation.PushModalAsync(modalPage);
            }
            else
                await DisplayAlert("Alert", "You have chosen not to log out.", "OK");
        }
    }
}
