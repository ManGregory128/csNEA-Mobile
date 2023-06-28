using Microsoft.Data.SqlClient;
using ParonApp.Views;

namespace ParonApp;

public partial class LoginPage : ContentPage
{
    List<User> listOfUsers = new List<User>() { };
    User tempUser;

    public LoginPage()
    {
        InitializeComponent();
        if (Preferences.ContainsKey("DBaddress") && Preferences.ContainsKey("DBpassword"))
        {
            entAddress.Text = Preferences.Default.Get("DBaddress", "Database Address");
            entDBPass.Text = Preferences.Default.Get("DBpassword", "Database Password");
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        bool success = false;
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        builder.DataSource = entAddress.Text;
        builder.UserID = "SA";
        builder.Password = entDBPass.Text;
        builder.InitialCatalog = "kiti";
        builder.TrustServerCertificate = true;

        Preferences.Default.Set("DBaddress", entAddress.Text);
        Preferences.Default.Set("DBpassword", entDBPass.Text);
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
                    Preferences.Default.Set("CurrentUser", listOfUsers[i].username);
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
                    Preferences.Default.Set("firstRun", false);
                    break;
                }
            }
            if (success == false)
            {
                ShowMessage(1);
                listOfUsers.Clear();
            }
            else
                await Shell.Current.GoToAsync("//mainPage");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Alert", ex.Message, "OK");
            //ShowMessage(2);
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

    private async void btnDemo_Clicked(object sender, EventArgs e)
    {
        Preferences.Default.Set("demo", true);
        await Shell.Current.GoToAsync("//mainPage");
    }
}
public class User
{
    public string username, password, firstName;
}
