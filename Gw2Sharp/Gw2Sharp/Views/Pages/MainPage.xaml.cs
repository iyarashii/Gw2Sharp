using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Xamarin.Forms;
//using System.Net.Http;
//using Gw2Sharp.Models;

namespace Gw2Sharp.Views.Pages
{
 
    public partial class MainPage : ContentPage
    {
        //public static InternetConnection Connection = new InternetConnection();
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
