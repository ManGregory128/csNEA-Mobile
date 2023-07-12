using ParonApp.Views;
using System.ServiceModel;
using Microsoft.IdentityModel.Protocols.WsTrust;
using System.Globalization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ParonApp;

public partial class LoginPage : ContentPage
{

    public LoginPage()
    {
        InitializeComponent();
    }
    private async void Button_Clicked(object sender, EventArgs e)
    {
        activityIndicator.IsRunning = true;
        bool success = await LoginMobile(entUserName.Text, entPasswd.Text);
        activityIndicator.IsRunning = false;
        if (success)
        {
            Preferences.Default.Set("loggedIn", true);
            Preferences.Default.Set("CurrentUser", entUserName.Text);
            entUserName.Text = "";
            entPasswd.Text = "";
            btnDemo.IsEnabled = true;
            await Shell.Current.GoToAsync("//mainPage");
        }
    }

    async void ShowMessage(int reason)
    {
        if (reason == 1) //the database info is correct but user does not exist or username and/or password is incorrect
            await DisplayAlert("Alert", "The Username and/or Password are incorrect.", "OK");
        else //the database could not be reached in the first place
            await DisplayAlert("Alert", "Could not connect to Paron's services.", "OK");
    }

    private async void btnDemo_Clicked(object sender, EventArgs e)
    {
        Preferences.Default.Set("demo", true);
        await Shell.Current.GoToAsync("//mainPage");
    }
    public async Task<bool> LoginMobile(string username, string password)
    {
        try
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.paron.app/api/User/loginMobile/" + entUserName.Text + "/" + entPasswd.Text);
            HttpResponseMessage response = await client.GetAsync("");
            if (response.IsSuccessStatusCode)
            {
                string content = response.Content.ReadAsStringAsync().Result;
                if (content == "true") return true;
                else
                {
                    ShowMessage(1);
                    return false;
                }
            }
            ShowMessage(0);
            return false;
        }
        catch
        {
            ShowMessage(0);
            return false;
        }
    }
}
