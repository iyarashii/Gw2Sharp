using System;
using Xamarin.Forms;

namespace Gw2Sharp.Views.Pages
{

    public partial class MainPage : ContentPage
    {
        public MainPage()
        {                   
            InitializeComponent();           
        }
        async void OnGemExchange(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GemExchangePage());
        }
        async void OnTradingPost(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TradingPostPage());
        }
        async void OnSettings(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ConfigurationPage());
        }
    }
}
