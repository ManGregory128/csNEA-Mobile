using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
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
            InitializeComponent();
            if (Settings.FirstRun == true)
            {
                LoginScreen modalPage = new LoginScreen();
                this.Navigation.PushModalAsync(modalPage);
                frameMsg.Text = "Good morning!";
            }
            else
            {
                UpdateFrame();
            }
                                                
            //DateTime localDate = DateTime.Now;
            //BindingContext = new InitializationClass();
        }

        public static void SetDBinfo(string input)
        {
            Settings.builder = new SqlConnectionStringBuilder
            {
                DataSource = input,
                UserID = "SA",
                Password = "]JKfpLZSp=8Qd*NM",
                InitialCatalog = "attendanceDB"
            };
        }

        private void UpdateFeed()
        {
            
        }

        public void UpdateFrame()
        {
            frameMsg.Text = "Good morning, " + Settings.CurrentUsername + "!";           
        }

        async void BtnChangePasswd_Clicked(object sender, EventArgs e)
        {
            if (Settings.CurrentPassword == entCurrentPasswd.Text)
            {
                if (entNewPasswd.Text == entConfirmPasswd.Text)
                {
                    using (SqlConnection connection = new SqlConnection(Settings.builder.ConnectionString))
                    {
                        String sql = "UPDATE Users SET UserPassword='" + entConfirmPasswd.Text + "' WHERE UserName='" + Settings.CurrentUsername + "';";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                    Settings.CurrentPassword = entConfirmPasswd.Text;
                    await DisplayAlert("Success", "Password has been saved.", "OK");
                    entCurrentPasswd.Text = "";
                    entNewPasswd.Text = "";
                    entConfirmPasswd.Text = "";
                }
                else
                    await DisplayAlert("Alert", "Passwords Must Match!", "OK");
            }
            else
                await DisplayAlert("Alert", "The current password is incorrect.", "OK");
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
                String sql = "UPDATE dbo.Users SET IsLoggedIn = 0 WHERE UserName = '" + Settings.CurrentUsername + "';";
                using (SqlConnection connection = new SqlConnection(Settings.builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
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
