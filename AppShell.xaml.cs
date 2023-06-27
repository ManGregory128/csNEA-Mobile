using ParonApp.Views;
using System.Windows.Input;

namespace ParonApp;

public partial class AppShell : Shell
{
    public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();
    public ICommand HelpCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));

    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
        BindingContext = this;
    }

    void RegisterRoutes()
    {
        Routes.Add("mainPage", typeof(MainPage));
        Routes.Add("feedPage", typeof(FeedPage));
        Routes.Add("settingsPage", typeof(SettingsPage));
        Routes.Add("loginPage", typeof(LoginPage));
        Routes.Add("attendancePage", typeof(AttendancePage));

        foreach (var item in Routes)
        {
            Routing.RegisterRoute(item.Key, item.Value);
        }
    }
}
