using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Gw2Sharp.Schemas;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace Gw2Sharp.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ConfigurationPage : ContentPage
	{
        // property used for storing number of item api pages
        public int MaxApiPages { get; set; }

        // property used as a flag, that shows whether app is currently sending GET requests to the GW2 API
        public bool GettingApiResponses { get; set; } = false;

        // page constructor
        public ConfigurationPage()
		{
			InitializeComponent();
		}

        // override that changes back button behavior depending on GettingApiResponses property value
        protected override bool OnBackButtonPressed()
        {
            if (GettingApiResponses)
            {
                statusText.Text = "Can't go back while getting data from api!";
                return true;
            }
            return false;
        }

        // method that gets the current number of max api pages from api
        async void GetApiMaxPages()
        {
            string apiPagesLink = @"https://api.guildwars2.com/v2/items?page=-1&page_size=200";
            string contentString;
            HttpResponseMessage apiPagesResponse;

            try
            {
                apiPagesResponse = await InternetConnection.client.GetAsync(apiPagesLink);
            }
            catch (HttpRequestException)
            {
                statusText.Text = "Http request error!";
                BindingContext = this;
                return;
            }
            catch (Exception)
            {
                statusText.Text = "Unknown exception!";
                BindingContext = this;
                return;
            }

            contentString = await apiPagesResponse.Content.ReadAsStringAsync();
            contentString = Regex.Match(contentString, @"\d+(?=\.)").Value;
            if (!int.TryParse(contentString, out int maxApiPages))
            {
                statusText.Text = "Error occured while parsing to int!";
                //statusText.Text = contentString;
                BindingContext = this;
                return;
            }
            MaxApiPages = maxApiPages;
        }

        // event handler that saves item name & id values from api to a local file on saveItemDB button click
        async void OnSaveItemDB(object sender, EventArgs e)
        {
            // check if internet connection is available
            //if (!MainPage.Connection.CheckForInternetConnection(statusText)) return;
            
            // set a flag that tells GET request are being send
            GettingApiResponses = true;

            // enable stop button
            stopButton.IsEnabled = true;

            // get number of max api pages
            GetApiMaxPages();

            // get item names and ids; check if it was successful
            bool getRequestApiResponseSuccess = await GetItemNamesAndIds();
            if (!getRequestApiResponseSuccess) return;

            // change button text
            saveItemDB.Text = "Done! Click again to redownload and overwrite local database file";

            // change flag to signal that GET requests are no longer being send
            GettingApiResponses = false;

            // set binding context to show changes in UI
            BindingContext = this;
        }

        // method that asynchronously sends GET requests to the api to receive 200 JSONs per request
        async Task<bool> GetItemNamesAndIds()
        {
            string apiResponse = null;
            saveItemDB.Text = "Getting api responses in progress... ";

            for (int i = 0; i <= MaxApiPages; ++i)
            {
                if (!GettingApiResponses)
                {
                    saveItemDB.Text = "Stopped at item page " + (i - 1) + "! Click again to redownload item info.";
                    BindingContext = this;
                    return false;
                }
                string apiItemLink = "https://api.guildwars2.com/v2/items?page=" + i + "&page_size=200";
                try
                {
                    apiResponse = await InternetConnection.client.GetStringAsync(apiItemLink);
                }
                catch (HttpRequestException)
                {
                    statusText.Text = "Http request error!";
                    BindingContext = this;
                    return false;
                }
                catch (Exception)
                {
                    statusText.Text = "Unknown exception!";
                    BindingContext = this;
                    return false;
                }
                saveItemDB.Text = "Getting api responses in progress... " + "(" + i + "/" + MaxApiPages + ")";
                BindingContext = this;
                DeserializeAndAppendToFile(apiResponse);
            }
            return true;
        }

        //  deserializes JSONs and appends ids and names from them to a text file
        void DeserializeAndAppendToFile(string apiResponse)
        {
            string itemDatabase = null;
            List<ItemNamesAndIds> itemNamesAndIds = JsonConvert.DeserializeObject<List<ItemNamesAndIds>>(apiResponse);
            for (int x = 0; x < itemNamesAndIds.Count; x++)
            {
                itemDatabase += itemNamesAndIds[x].id;
                itemDatabase += " " + itemNamesAndIds[x].name + "\n";
            }
            File.AppendAllText(TradingPostPage.ItemDBPath, itemDatabase);
            itemDatabase = null;
        }

        // event handler for deleteItemDB button that deletes local file that stores item name & id values from api
        void OnDeleteItemDB(object sender, EventArgs e)
        {
            try
            {
                File.SetAttributes(TradingPostPage.ItemDBPath, FileAttributes.Normal);
                File.Delete(TradingPostPage.ItemDBPath);
            }
            catch (FileNotFoundException)
            {
                statusText.Text = "Database not found!";
                BindingContext = this;
                return;
            }
            statusText.Text = "Database file successfully deleted!";
            BindingContext = this;
        }

        // event handler for stop button; changes GettingApiResponses value to prevent sending GET requests to api
        void OnStopButton(object sender, EventArgs e)
        {
            GettingApiResponses = false;
            stopButton.IsEnabled = false;
        }
    }
}