using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ParonApp.Views;

public partial class FeedPage : ContentPage
{
    private bool _isRefreshing = false;
    ObservableCollection<FeedPost> Posts;
    public FeedPage()
	{
		InitializeComponent();
        BindingContext = this;
        if (Posts == null) UpdateFeed();
	}
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
    private async void UpdateFeed()
    {
        Posts = new ObservableCollection<FeedPost>();
        
        try
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.paron.app/api/FeedPost/");
            HttpResponseMessage response = await client.GetAsync("");
            if (response.IsSuccessStatusCode)
            {
                string content = response.Content.ReadAsStringAsync().Result;
                Posts = JsonConvert.DeserializeObject<ObservableCollection<FeedPost>>(content);
                lstFeed.ItemsSource = Posts;
            }
        }
        catch
        {
            await DisplayAlert("Alert", "Cannot fetch the Feed. Make sure you are connected to a network.", "OK");
        }
    }
}