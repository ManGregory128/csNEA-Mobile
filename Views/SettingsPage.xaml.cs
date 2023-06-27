using Microsoft.Data.SqlClient;

namespace ParonApp.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}
    async void BtnChangePasswd_Clicked(object sender, EventArgs e)
    {
        /*
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
                        String sql = "UPDATE Users SET UserPassword='" + entConfirmPasswd.Text + "' WHERE UserName='" + Preferences.Default.Get("CurrentUser", "user") + "';";

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
        */
    }
    private void BtnLogOut_Clicked(object sender, EventArgs e)
    {
        LogOutAsync();
    }
    private async void LogOutAsync()
    {
        /*
        logoutConfrm = await DisplayAlert("Alert!", "Do you really want to log out?", "Yes", "No");

        if (logoutConfrm == true)
        {
            String sql = "UPDATE dbo.Users SET IsLoggedIn = 0 WHERE UserName = '" + Preferences.Default.Get("CurrentUser", "user") + "';";
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
            Preferences.Set("firstRun", true);
            ShowLogin();
        }
        else
            ShowErrorMessage(3);
        */
    }
}