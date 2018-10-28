using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;

namespace Gw2Sharp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GemExchangePage : ContentPage
	{
        public string GemInfo { get; set; }

        private static readonly HttpClient client = new HttpClient();

        public GemExchangePage()
		{
            InitializeComponent();
            GemInfo = "Current gem to gold exchange:";
            BindingContext = this;
        }
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        
        async void OnCalculateGoldToGems(object sender, EventArgs e)
        {
            BindingContext = null;

            if (!CheckForInternetConnection())
            {
                GemInfo = "No internet connection!";
                BindingContext = this;
                return;
            }

            GemInfo = "Current gem to gold exchange:\n";     
            
            if (!double.TryParse(goldAmount.Text, out double coins))
            {
                GemInfo = goldAmount.Text + " is not a number.";
                BindingContext = this;
                return;
            }
            coins = Convert.ToDouble(goldAmount.Text) * 10000.0;            
            //GemInfo += coins.ToString() + "\n";            
            string apiResponse;
            string apiGemLink = "https://api.guildwars2.com/v2/commerce/exchange/coins?quantity=" + coins;
            //var json = new WebClient().DownloadString(apiGemLink);   
            
            try
            {
                var responseString = await client.GetStringAsync(apiGemLink);
                apiResponse = responseString;
                /*
                using (WebClient wc = new WebClient())
                {
                    var json = wc.DownloadString(apiGemLink);                     
                    apiResponse = json;
                }
                */
            }
            catch (HttpRequestException)
            {                
                GemInfo =  coins > 10000.0 ? "too many coins!" : "too few coins!";
                BindingContext = this;
                return;
            }            
            catch (Exception)
            {
                GemInfo += "Unknown exception!";
                BindingContext = this;
                return;
            }
            GemExchangeRate gemResponse = JsonConvert.DeserializeObject<GemExchangeRate>(apiResponse);
            
            GemInfo += "Send: " + coins / 10000.0 + " gold\n";
            GemInfo += "To receive: " + gemResponse.Quantity + " gems\n";
            double goldPerGemRatio = gemResponse.Coins_per_gem / 10000.0;
            GemInfo += "Gold per gem: " + goldPerGemRatio + "\n";
            GemInfo += "For 400 gems you have to pay about " + goldPerGemRatio * 400 + " gold\n";
            GemInfo += "For 800 gems you have to pay about " + goldPerGemRatio * 800 + " gold\n";
            GemInfo += "For 1200 gems you have to pay about " + goldPerGemRatio * 1200 + " gold\n";
            GemInfo += "For 2000 gems you have to pay about " + goldPerGemRatio * 2000 + " gold\n";
            
            BindingContext = this;
        }
    }
}