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
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            bool success = false;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "192.168.0.30";
            builder.UserID = "SA";
            builder.Password = "CYrulis2002";
            builder.InitialCatalog = "attendanceDB";

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
                            //Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
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
                    listOfUsers.Clear();
                    Settings.FirstRun = false;
                }
            }
            if (success == false)
            {
                ShowMessage();
                listOfUsers.Clear();
            }
            else
                this.Navigation.PopModalAsync();
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
        async void ShowMessage()
        {
            await DisplayAlert("Alert", "The Username and/or Password are incorrect.", "OK");
        }
    }
}