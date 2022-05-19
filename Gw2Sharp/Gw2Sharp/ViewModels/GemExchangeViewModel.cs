// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Gw2Sharp/blob/master/LICENSE for license details.

using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Gw2Sharp.Models.DTOs;
using Gw2Sharp.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace Gw2Sharp.ViewModels
{
    public class GemExchangeViewModel : BaseViewModel
    {
        // properties used for storing text values for different Labels
        #region BindingProperties
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
        #endregion

        // stores deserialized data from API response
        public GemExchangeRate ApiResponse { get; set; }

        // command properties
        public Command CalculateGoldToGemsCommand { get; set; }

        // constructor
        public GemExchangeViewModel()
        {
            // hide responseTextLayout and status text
            IsResponseLayoutVisible = false;
            IsGemToGoldExchangeStatusTextVisible = false;

            // create command for asynchronous execute method
            CalculateGoldToGemsCommand = new Command(async () => await ExecuteCalculateGoldToGemsCommand());
        }

        // calculates and displays gold to gem exchange rate
        async Task ExecuteCalculateGoldToGemsCommand()
        {

            // hide responseTextLayout to hide images
            IsResponseLayoutVisible = false;

            // hide status text
            IsGemToGoldExchangeStatusTextVisible = false;

            // set status text value
            GemToGoldExchangeStatusText = "Current gem to gold exchange:";

            // check internet connection
            GemToGoldExchangeStatusText = InternetConnection.CheckForInternetConnection(GemToGoldExchangeStatusText);
            if (!InternetConnection.Connection)
            {
                IsGemToGoldExchangeStatusTextVisible = true;
                return;
            }

            // set parsed value from entry to coins variable
            double coins = CheckIfGoldAmountIsValid();

            // get API response and check if it was a success
            bool apiResponseSuccess = await GetApiResponse(coins);

            // set status text visibility to true to show possible errors
            IsGemToGoldExchangeStatusTextVisible = true;

            // if API response returned error end method execution here
            if (!apiResponseSuccess) return;

            // show responseTextLayout to show images and values from API
            IsResponseLayoutVisible = true;

            // assign received values from API to the correct properties 
            SetValuesFromApiData(coins);
        }

        // assigns values from API to labels text properties
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

        // tries to parse goldAmount entry value to double and returns parsed value
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

        // gets api response string and deserializes it
        async Task<bool> GetApiResponse(double coins)
        {

            string apiResponseString;
            string apiGemLink = "https://api.guildwars2.com/v2/commerce/exchange/coins?quantity=" + coins;

            // don't show the error message if goldAmount entry value is not a number
            if (coins == -1)
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
