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

namespace Gw2Sharp.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GemExchangePage : ContentPage
	{

        // properties used for storing text values for different Labels
        public string GemToGoldExchangeStatusText { get; set; }
        public string SendGoldText { get; set; }
        public string ReceiveGemText { get; set; }
        public string GoldPerGemText { get; set; }
        public string PriceOf400Gems { get; set; }
        public string PriceOf800Gems { get; set; }
        public string PriceOf1200Gems { get; set; }
        public string PriceOf2000Gems { get; set; }

        // property that stores deserialized data from api response
        public GemExchangeRate ApiResponse { get; set; }

        // property that stores text height value, used for changing gold and gem icons to the same height as text
        public double TextHeight { get { return gemToGoldExchangeStatusText.Height; } }


        // page constructor
        public GemExchangePage()
		{
            InitializeComponent();
        }

        // event handler that calculates and shows gold to gem exchange rate when calculateGoldToGem button is clicked
        async void OnCalculateGoldToGems(object sender, EventArgs e)
        {
            // reset binding context so that changes made to binding values can be seen after pressing button multiple times
            BindingContext = null;

            // hide responseTextLayout to hide images
            responseTextLayout.IsVisible = false;

            // check internet connection
            if (!MainPage.Connection.CheckForInternetConnection(gemToGoldExchangeStatusText)) return;
            
            // set status text value
            GemToGoldExchangeStatusText = "Current gem to gold exchange:";

            // set parsed value from entry to coins variable
            double coins = CheckIfGoldAmountIsValid();

            // get api response and check if it was a success
            bool apiResponseSuccess = await GetApiResponse(coins);
            if (apiResponseSuccess == false) return;

            // show responseTextLayout to show images and values from api
            responseTextLayout.IsVisible = true;

            // assign received values from api to the correct properties 
            SetValuesFromApiData(coins);
            // set binding context to this page to save changes made to labels through the properties
            BindingContext = this;
        }

        // method that assigns values from api to label.text properties
        void SetValuesFromApiData(double coins)
        {
            SendGoldText = "Send: " + coins / 10000.0;
            ReceiveGemText = "To receive: " + ApiResponse.Quantity;
            double goldPerGemRatio = ApiResponse.Coins_per_gem / 10000.0;
            GoldPerGemText = "Gold per gem: " + goldPerGemRatio;
            PriceOf400Gems = "you have to pay about " + goldPerGemRatio * 400;
            PriceOf800Gems = "you have to pay about " + goldPerGemRatio * 800;
            PriceOf1200Gems = "you have to pay about " + goldPerGemRatio * 1200;
            PriceOf2000Gems = "you have to pay about " + goldPerGemRatio * 2000;
        }

        // method that tries parsing goldAmount entry value to double and returns parsed value
        double CheckIfGoldAmountIsValid()
        {
            if (!double.TryParse(goldAmount.Text, out double coins))
            {
                GemToGoldExchangeStatusText = goldAmount.Text + " is not a number.";
                BindingContext = this;
                return -1;
            }
            
            coins = coins * 10000.0;
            return coins;
        }

        // async method that gets api response string and deserializes it
        async Task<bool> GetApiResponse(double coins)
        {

            string apiResponseString;
            string apiGemLink = "https://api.guildwars2.com/v2/commerce/exchange/coins?quantity=" + coins;

            try
            {
                apiResponseString = await InternetConnection.client.GetStringAsync(apiGemLink);
            }
            catch (HttpRequestException)
            {
                GemToGoldExchangeStatusText =  coins > 10000.0 ? "too many coins!" : "too few coins!";
                BindingContext = this;
                return false;
            }            
            catch (Exception)
            {
                GemToGoldExchangeStatusText = "Unknown exception!";
                BindingContext = this;
                return false;
            }
            ApiResponse = JsonConvert.DeserializeObject<GemExchangeRate>(apiResponseString);
            return true;
        }
    }
}