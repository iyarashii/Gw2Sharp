using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Gw2Sharp.Models.DTOs;
using Gw2Sharp.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Input;

namespace Gw2Sharp.ViewModels
{
    public class ConfigurationViewModel : BaseViewModel
    {
        // property used for storing number of item api pages
        public int MaxApiPages { get; set; }

        // property used as a flag, that shows whether app is currently sending GET requests to the GW2 API
        public bool GettingApiResponses { get; set; }
        public string SaveItemDBButtonText { get; set; } = "Save item name and id list";
        public string ConfigurationStatusText { get; set; }

        public ICommand SaveItemDBCommand { get; set; }
        public ICommand DeleteItemDBCommand { get; set; }
        public ICommand StopButtonCommand { get; set; }

        public ConfigurationViewModel()
        {
            GettingApiResponses = false;
            // create command for asynchronous method
            SaveItemDBCommand = new Command(async () => await ExecuteSaveItemDBCommand());
            DeleteItemDBCommand = new Command( () =>  ExecuteDeleteItemDBCommand());
            StopButtonCommand = new Command( () =>  ExecuteStopButtonCommand(), () => 
            {
                //if (!GettingApiResponses) return false;
                //else return true;
                return GettingApiResponses;
            });
            (StopButtonCommand as Command).ChangeCanExecute();
        }

        // method that gets the current number of max api pages from api
        async Task GetApiMaxPages()
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
                ConfigurationStatusText = "Http request error!";
                return;
            }
            catch (Exception)
            {
                ConfigurationStatusText = "Unknown exception!";
                return;
            }

            contentString = await apiPagesResponse.Content.ReadAsStringAsync();
            contentString = Regex.Match(contentString, @"\d+(?=\.)").Value;
            if (!int.TryParse(contentString, out int maxApiPages))
            {
                ConfigurationStatusText = "Error occured while parsing to int!";
                //statusText.Text = contentString;
                return;
            }
            MaxApiPages = maxApiPages;
        }

        // event handler that saves item name & id values from api to a local file on saveItemDB button click
        async Task ExecuteSaveItemDBCommand()
        {
            // check internet connection
            ConfigurationStatusText = InternetConnection.CheckForInternetConnection(ConfigurationStatusText);
            if (!InternetConnection.connection)
            {
                return;
            }

            // set a flag that tells GET request are being send
            GettingApiResponses = true;

            // enable stop button
            (StopButtonCommand as Command).ChangeCanExecute();

            //IsStopButtonEnabled = true;

            // get number of max api pages
            await GetApiMaxPages();

            // get item names and ids; check if it was successful
            bool getRequestApiResponseSuccess = await GetItemNamesAndIds();
            if (!getRequestApiResponseSuccess) return;

            // change button text
            SaveItemDBButtonText = "Done! Click again to redownload and overwrite local database file";

            // change flag to signal that GET requests are no longer being send
            GettingApiResponses = false;
        }

        // method that asynchronously sends GET requests to the api to receive 200 JSONs per request
        async Task<bool> GetItemNamesAndIds()
        {
            string apiResponse;
            SaveItemDBButtonText = "Getting api responses in progress... ";
            string apiItemLink;
            for (int i = 0; i <= MaxApiPages; ++i)
            {
                if (!GettingApiResponses)
                {
                    SaveItemDBButtonText = "Stopped at item page " + (i - 1) + "! Click again to redownload item info.";
                    return false;
                }
                apiItemLink = "https://api.guildwars2.com/v2/items?page=" + i + "&page_size=200";
                try
                {
                    apiResponse = await InternetConnection.client.GetStringAsync(apiItemLink);
                }
                catch (HttpRequestException)
                {
                    ConfigurationStatusText = "Http request error!";
                    return false;
                }
                catch (Exception)
                {
                    ConfigurationStatusText = "Unknown exception!";
                    return false;
                }
                SaveItemDBButtonText = "Getting api responses in progress... " + "(" + i + "/" + MaxApiPages + ")";
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
            File.AppendAllText(Constants.ItemDBPath, itemDatabase);
        }

        // method that deletes local file that stores item name & id values from api
        void ExecuteDeleteItemDBCommand()
        {
            try
            {
                File.SetAttributes(Constants.ItemDBPath, FileAttributes.Normal);
                File.Delete(Constants.ItemDBPath);
            }
            catch (FileNotFoundException)
            {
                ConfigurationStatusText = "Database not found!";
                return;
            }
            ConfigurationStatusText = "Database file successfully deleted!";
        }

        // method that changes GettingApiResponses value to prevent sending GET requests to api
        void ExecuteStopButtonCommand()
        {
           GettingApiResponses = false;
            (StopButtonCommand as Command).ChangeCanExecute();
        }
    }
}
