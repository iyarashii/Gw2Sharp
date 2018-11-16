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
using Gw2Sharp.Schemas;

namespace Gw2Sharp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GemExchangePage : ContentPage
	{
        public string GemToGoldExchangeStatusText { get; set; }
        public string SendGoldText { get; set; }
        public string ReceiveGemText { get; set; }
        public string GoldPerGemText { get; set; }
        public string PriceOf400Gems { get; set; }
        public string PriceOf800Gems { get; set; }
        public string PriceOf1200Gems { get; set; }
        public string PriceOf2000Gems { get; set; }
        public double TextHeight { get { return gemToGoldExchangeStatusText.Height; } }



        public GemExchangePage()
		{
            InitializeComponent();
            GemToGoldExchangeStatusText = "Current gem to gold exchange:";
            BindingContext = this;
        }

       
        async void OnCalculateGoldToGems(object sender, EventArgs e)
        {
            BindingContext = null;
            responseTextLayout.IsVisible = false;

            if (!InternetConnection.CheckForInternetConnection())
            {
                GemToGoldExchangeStatusText = "No internet connection!";
                BindingContext = this;
                return;
            }

            GemToGoldExchangeStatusText = "Current gem to gold exchange:";     
            
            if (!double.TryParse(goldAmount.Text, out double coins))
            {
                GemToGoldExchangeStatusText = goldAmount.Text + " is not a number.";
                BindingContext = this;
                return;
            }
            coins = Convert.ToDouble(goldAmount.Text) * 10000.0;            
                     
            string apiResponse;
            string apiGemLink = "https://api.guildwars2.com/v2/commerce/exchange/coins?quantity=" + coins;
  
            
            try
            {
                apiResponse = await InternetConnection.client.GetStringAsync(apiGemLink);               
            }
            catch (HttpRequestException)
            {
                GemToGoldExchangeStatusText =  coins > 10000.0 ? "too many coins!" : "too few coins!";
                BindingContext = this;
                return;
            }            
            catch (Exception)
            {
                GemToGoldExchangeStatusText = "Unknown exception!";
                BindingContext = this;
                return;
            }
            GemExchangeRate gemResponse = JsonConvert.DeserializeObject<GemExchangeRate>(apiResponse);

            responseTextLayout.IsVisible = true;

            SendGoldText = "Send: " + coins / 10000.0;
            ReceiveGemText = "To receive: " + gemResponse.Quantity;
            double goldPerGemRatio = gemResponse.Coins_per_gem / 10000.0;
            GoldPerGemText = "Gold per gem: " + goldPerGemRatio;
            PriceOf400Gems = "you have to pay about " + goldPerGemRatio * 400;
            PriceOf800Gems = "you have to pay about " + goldPerGemRatio * 800;
            PriceOf1200Gems = "you have to pay about " + goldPerGemRatio * 1200;
            PriceOf2000Gems = "you have to pay about " + goldPerGemRatio * 2000;
            BindingContext = this;
        }
    }
}