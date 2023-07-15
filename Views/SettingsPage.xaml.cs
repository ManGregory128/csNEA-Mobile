
namespace ParonApp.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }
    async void BtnChangePasswd_Clicked(object sender, EventArgs e)
    {
        string currentPassword = entCurrentPasswd.Text;

        if (entNewPasswd.Text == "" || entConfirmPasswd.Text == "")
        {
            await DisplayAlert("Alert", "New password cannot be empty.", "OK");
        }
        else
        {
            if (entNewPasswd.Text == entConfirmPasswd.Text)
            {
                try
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("https://api.paron.app/api/User/changePassword/" +
                        Preferences.Default.Get("CurrentUser", "") + "/" + currentPassword + "/" + entConfirmPasswd.Text);
                    HttpResponseMessage response = await client.GetAsync("");
                    if (response.IsSuccessStatusCode)
                    {
                        string content = response.Content.ReadAsStringAsync().Result;
                        if (content == "true")
                        {
                            await DisplayAlert("Alert", "Password change was successfull.", "OK");
                        }
                        else await DisplayAlert("Alert", "Password change was NOT successfull.", "OK");

                    }
                    else await DisplayAlert("Alert", "Password change was NOT successfull.", "OK");
                }
                catch
                {
                    await DisplayAlert("Alert", "Password change was NOT successfull.", "OK");
                }
            }
            else
                await DisplayAlert("Alert", "New password does not match with confirmed new password.", "OK");
        }
    }
    private async void BtnLogOut_Clicked(object sender, EventArgs e)
    {
        if (await LogOutAsync())
        {
            Preferences.Set("loggedIn", false);
            await Shell.Current.GoToAsync("//loginPage");
        }
    }
    private async Task<bool> LogOutAsync()
    {
        string password = await DisplayPromptAsync("To confirm log out, please enter your password:", "Password");
        if (password != null)
        {
            bool logoutConfrm = await DisplayAlert("Alert!", "Do you really want to log out?", "Yes", "No");
            if (logoutConfrm)
            {
                try
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("https://api.paron.app/api/User/logoutMobile/" +
                        Preferences.Default.Get("CurrentUser", "") + "/" + password);
                    HttpResponseMessage response = await client.GetAsync("");
                    if (response.IsSuccessStatusCode)
                    {
                        string content = response.Content.ReadAsStringAsync().Result;
                        if (content == "true")
                        {
                            return true;
                        }
                        else return false;

                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }
        return false;
    }
}