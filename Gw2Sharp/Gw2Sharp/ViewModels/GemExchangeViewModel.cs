using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
//using Gw2Sharp.Views.Pages;
using Gw2Sharp.Models.DTOs;
using Gw2Sharp.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace Gw2Sharp.ViewModels
{
    public class GemExchangeViewModel : BaseViewModel
    {
        // field storing whether internet connection is working
        bool internetConnection;

        // properties used for storing text values for different Labels
        public string GemToGoldExchangeStatusText { get; set; }
        public string SendGoldText { get; set; }
        public string ReceiveGemText { get; set; }
        public string GoldPerGemText { get; set; }
        public string PriceOf400Gems { get; set; }
        public string PriceOf800Gems { get; set; }
        public string PriceOf1200Gems { get; set; }
        public string PriceOf2000Gems { get; set; }
        public string GoldAmount { get; set; }

        // property that stores bool value for response layout visibility
        public bool IsResponseLayoutVisible { get; set; }

        // property that stores bool value for status text visibility
        public bool IsGemToGoldExchangeStatusTextVisible { get; set; }

        // property that stores deserialized data from api response
        public GemExchangeRate ApiResponse { get; set; }

        // command properties
        public Command CalculateGoldToGemsCommand { get; set; }

        // constructor
        public GemExchangeViewModel()
        {
            // hide responseTextLayout and status text
            IsResponseLayoutVisible = false;
            IsGemToGoldExchangeStatusTextVisible = false;

            // create command for asynchronous method
            CalculateGoldToGemsCommand = new Command(async () => await ExecuteCalculateGoldToGemsCommand());
        }

        // method that calculates and shows gold to gem exchange rate when calculateGoldToGem button is clicked
        async Task ExecuteCalculateGoldToGemsCommand()
        {

            // hide responseTextLayout to hide images
            IsResponseLayoutVisible = false;

            // hide status text
            IsGemToGoldExchangeStatusTextVisible = false;

            // set status text value
            GemToGoldExchangeStatusText = "Current gem to gold exchange:";

            // check internet connection
            GemToGoldExchangeStatusText = InternetConnection.CheckForInternetConnection(GemToGoldExchangeStatusText, ref internetConnection);
            if (!internetConnection)
            {
                IsGemToGoldExchangeStatusTextVisible = true;
                return;
            }

            // set parsed value from entry to coins variable
            double coins = CheckIfGoldAmountIsValid();

            // get api response and check if it was a success
            bool apiResponseSuccess = await GetApiResponse(coins);

            // set status text visibilty to true to show possible errors
            // set status text visibilty to true to show possible errors
            IsGemToGoldExchangeStatusTextVisible = true;

            // if api response returned error end method execution here
            if (apiResponseSuccess == false) return;

            // show responseTextLayout to show images and values from api
            IsResponseLayoutVisible = true;

            // assign received values from api to the correct properties 
            SetValuesFromApiData(coins);
            return;
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
            if (!double.TryParse(GoldAmount, out double coins))
            {
                GemToGoldExchangeStatusText = GoldAmount + " is not a number.";
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

            // dont show error message if
            if(coins == -1)
            {
                return false;
            }

            try
            {
                apiResponseString = await InternetConnection.client.GetStringAsync(apiGemLink);
            }
            catch (HttpRequestException)
            {
                GemToGoldExchangeStatusText = coins > 10000.0 ? "too many coins!" : "too few coins!";
                return false;
            }
            catch (Exception)
            {
                GemToGoldExchangeStatusText = "Unknown exception!";
                return false;
            }
            ApiResponse = JsonConvert.DeserializeObject<GemExchangeRate>(apiResponseString);
            return true;
        }
    }
}
