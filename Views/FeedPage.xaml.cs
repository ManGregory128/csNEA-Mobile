using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ParonApp.Views;

public partial class FeedPage : ContentPage
{
    private bool _isRefreshing = false;
    ObservableCollection<FeedPost> posts;
    public FeedPage()
	{
		InitializeComponent();
        //if (posts.Count == 0) UpdateFeed();
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
    private void UpdateFeed()
    {
        posts = new ObservableCollection<FeedPost>();
        FeedPost tempPost;
        using (SqlConnection connection = new SqlConnection(""))
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
            //lstFeed.ItemsSource = posts;
        }
    }
}