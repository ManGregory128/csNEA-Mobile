using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace csNEA_mobileApp
{
    public partial class MainPage : TabbedPage
    {
        public ObservableCollection<FeedPost> posts { get; set; }
        public static SqlConnectionStringBuilder builder { get; set; }
        private bool _isRefreshing = false;
        public static List<int> CurrentPeriods { get; set; }
        public bool logoutConfrm { get; set; }
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
        public ICommand RefreshCommand
        {
            get
            {
                return new Command(() =>
                {
                    IsRefreshing = true;

                    UpdateFeed();

                    IsRefreshing = false;
                });
            }
        }
        public MainPage()
        {

            InitializeComponent();
            if (Settings.FirstRun == true)
            {
                ShowLogin();
                //frameMsg.Text = "Good morning!";
            }
            else
            {
                SetDBinfo();
                UpdateFrame();
                try
                {
                    UpdateFeed();
                    UpdatePeriods();
                }
                catch
                {
                    ShowErrorMessage(1);
                }
            }
            //DateTime localDate = DateTime.Now;
            this.BindingContext = this;
        }
        public async void ShowLogin()
        {
            LoginScreen modalPage = new LoginScreen();
            modalPage.Disappearing += (sender2, e2) =>
            {
                System.Diagnostics.Debug.WriteLine("The modal page is dismissed");
                UpdateFrame();
                UpdateFeed();
                UpdatePeriods();
            };
            await Navigation.PushModalAsync(modalPage);
        }
        public static void SetDBinfo()
        {
            builder = new SqlConnectionStringBuilder
            {
                DataSource = Settings.CurrentDatabase,
                UserID = "adminDB",
                Password = Settings.CurrentDBPassword,
                InitialCatalog = "aradippou5"
            };
        }

        private void UpdateFeed()
        {
            posts = new ObservableCollection<FeedPost>();
            posts.Clear();
            FeedPost tempPost;
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                String sql = "SELECT Author, DateTimePosted, Post FROM dbo.Feed ORDER BY DateTimePosted DESC;"; //Selecting Only from last week TODO

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //Assigning Posts to the List
                            tempPost = new FeedPost(reader.GetString(2), reader.GetString(0), reader.GetDateTime(1));
                            posts.Add(tempPost);
                        }
                        reader.Close();
                    }
                    connection.Close();
                }
                lstFeed.ItemsSource = posts;
            }
        }

        private void UpdatePeriods()
        {
            CurrentPeriods = new List<int>();
            int day = GetToday();
            List<string> periods = new List<string>();
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                String sql = "SELECT Teachings.PeriodID, Teachings.LessonID, Lessons.LessonName FROM dbo.Teachings " +
                    "INNER JOIN dbo.Lessons ON Teachings.LessonID = Lessons.LessonID WHERE TeacherUsername = '" + Settings.CurrentUsername + "' AND Day = " + day + ";";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            periods.Add("Period " + reader.GetByte(0).ToString() + " - " + reader.GetString(2));
                            //CurrentPeriods.Add(reader.GetInt32(0));
                        }
                        reader.Close();
                    }
                    connection.Close();
                }
            }
            pickerGroup.ItemsSource = periods;
        }
        private int GetToday()
        {
            DateTime today = DateTime.Now;
            string day = today.DayOfWeek.ToString();

            switch (day)
            {
                case "Monday":
                    return 1;
                case "Tuesday":
                    return 2;
                case "Wednesday":
                    return 3;
                case "Thursday":
                    return 4;
                case "Friday":
                    return 5;
                case "Saturday":
                    return 6;
                default:
                    return 7;
            }
        }

        public void UpdateFrame()
        {
            frameMsg.Text = "Good morning, " + Settings.CurrentUsername + "!";
        }

        async void BtnChangePasswd_Clicked(object sender, EventArgs e)
        {
            string currentPassword = GetPasswordCurrent();

            if (currentPassword == entCurrentPasswd.Text)
            {
                if (entNewPasswd.Text == "" || entConfirmPasswd.Text == "")
                {
                    await DisplayAlert("Alert", "New password cannot be empty.", "OK");
                }
                else
                {
                    if (entNewPasswd.Text == entConfirmPasswd.Text)
                    {
                        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                        {
                            String sql = "UPDATE Users SET UserPassword='" + entConfirmPasswd.Text + "' WHERE UserName='" + Settings.CurrentUsername + "';";

                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close();
                            }
                        }
                        await DisplayAlert("Success", "Password has been saved.", "OK");
                        entCurrentPasswd.Text = "";
                        entNewPasswd.Text = "";
                        entConfirmPasswd.Text = "";
                    }
                    else
                        await DisplayAlert("Alert", "Passwords Must Match!", "OK");
                }
            }
            else
                await DisplayAlert("Alert", "The current password is incorrect.", "OK");
        }

        private void BtnLogOut_Clicked(object sender, EventArgs e)
        {
            LogOutAsync();            
        }
        private async void LogOutAsync()
        {
            logoutConfrm = await DisplayAlert("Alert!", "Do you really want to log out?", "Yes", "No");

            if (logoutConfrm == true)
            {
                String sql = "UPDATE dbo.Users SET IsLoggedIn = 0 WHERE UserName = '" + Settings.CurrentUsername + "';";
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                ShowErrorMessage(2);
                Settings.FirstRun = true;
                ShowLogin();
            }
            else
                ShowErrorMessage(3);
        }
        async void BtnTakeAttendance_Clicked(object sender, EventArgs e)
        {
            int periodSelected;
            periodSelected = pickerGroup.SelectedIndex + 1;
            if (DateExists())
            {
                AttendancePage.PeriodPicked = periodSelected;
                AttendancePage attendancePage = new AttendancePage();
                await this.Navigation.PushModalAsync(attendancePage);
            }
            else
                await DisplayAlert("Alert", "There are no lessons for today.", "OK");
        }
        private bool DateExists()
        {
            DateTime today = DateTime.Now;
            int day;
            String sql = "SELECT COUNT(1) FROM dbo.Dates WHERE Date = '" + today.ToString("yyyy-MM-dd") + "';";
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    day = (int)command.ExecuteScalar();
                    connection.Close();
                }
            }
            if (day > 0)
            {
                return true;
            }
            else
                return false;
        }
        private string GetPasswordCurrent()
        {
            string password = "";
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                String sql = "SELECT UserPassword FROM dbo.Users WHERE UserName = '" + Settings.CurrentUsername + "';";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            password = reader.GetString(0);
                        }
                        reader.Close();
                    }
                    connection.Close();
                }
            }
            return password;
        }
        async void ShowErrorMessage(int reason)
        {
            if (reason == 1)
                await DisplayAlert("Alert", "Cannot contact the database. Make sure you are connected to a network.", "OK");
            else if (reason == 2)
                await DisplayAlert("Alert", "You have logged out. Restart of application is recommended.", "OK");
            else if (reason == 3)
                await DisplayAlert("Alert", "You have chosen not to log out.", "OK");
        }        
    }
}
