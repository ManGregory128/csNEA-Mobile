using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace csNEA_mobileApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginScreen : ContentPage
    {        
        List<User> listOfUsers = new List<User>() { };
        User tempUser;

        public LoginScreen()
        {           
            InitializeComponent();
            if (Settings.CurrentDatabase != String.Empty)
            {
                entAddress.Text = Settings.CurrentDatabase;
                entDBPass.Text = Settings.CurrentDBPassword;
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            bool success = false;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = entAddress.Text;
            builder.UserID = "adminDB";
            builder.Password = entDBPass.Text;
            builder.InitialCatalog = "aradippou5";

            Settings.CurrentDatabase = entAddress.Text;
            Settings.CurrentDBPassword = entDBPass.Text;
            try
            {
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    String sql = "SELECT UserName, UserPassword, FirstName FROM dbo.Users WHERE UserRole='t';"; //Selecting Teachers Only

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AddUser(reader.GetString(0), reader.GetString(1), reader.GetString(2));
                            }
                            reader.Close();
                        }
                        connection.Close();
                    }
                }
                for (int i = 0; i < listOfUsers.Count; i++)
                {
                    if (entUserName.Text == listOfUsers[i].username && entPasswd.Text == listOfUsers[i].password)
                    {
                        success = true;
                        Settings.CurrentUsername = listOfUsers[i].username;
                        MainPage.SetDBinfo();
                        String sql = "UPDATE dbo.Users SET IsLoggedIn = 1 WHERE UserName = '" + listOfUsers[i].username + "';";
                        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                        {
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                                connection.Close();
                            }
                        }

                        listOfUsers.Clear();
                        Settings.FirstRun = false;
                        break;
                    }
                }
                if (success == false)
                {
                    ShowMessage(1);
                    listOfUsers.Clear();
                }
                else
                    this.Navigation.PopModalAsync();
            }
            catch
            {
                ShowMessage(2);
            }
            
        }
        private void AddUser(string username, string password, string firstName)
        {
            tempUser = new User
            {
                username = username,
                password = password,
                firstName = firstName
            };
            listOfUsers.Add(tempUser);
        }
        async void ShowMessage(int reason)
        {
            if (reason == 1) //the database info is correct but user does not exist or username and/or password is incorrect
                await DisplayAlert("Alert", "The Username and/or Password are incorrect.", "OK");
            else //the database could not be reached in the first place
                await DisplayAlert("Alert", "Could not contact the Database.", "OK");
        }
    }
    public class User
    {
        public string username, password, firstName;
    }
}