using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;

namespace Gw2Sharp
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
    }
}
