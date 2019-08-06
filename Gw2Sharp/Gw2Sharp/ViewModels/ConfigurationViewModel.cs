using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Gw2Sharp.Models;
using Gw2Sharp.Models.DTOs;

namespace Gw2Sharp.ViewModels
{
    public class ConfigurationViewModel : BaseViewModel
    {
        // stores the number of item API pages
        public int MaxApiPages { get; set; }

        // stores a flag that shows whether the app is currently sending GET requests to the GW2 API
        public bool GettingApiResponses { get; set; } = false;

        // binding properties
        public string SaveItemDBButtonText { get; set; } = "Update local item name & id database";
        public string ConfigurationStatusText { get; set; }

        // command properties
        public Command SaveItemDBCommand { get; set; }
        public Command ClearItemDBCommand { get; set; }
        public Command StopButtonCommand { get; set; }

        // constructor
        public ConfigurationViewModel()
        {
            // create commands with their execute and canExecute methods
            SaveItemDBCommand = new Command(async () => await ExecuteSaveItemDBCommand(), () =>
            {
                return !GettingApiResponses;
            }
            );

            ClearItemDBCommand = new Command( () =>  ExecuteClearItemDBCommand(), () => 
            {
                return !GettingApiResponses;
            }
            );

            StopButtonCommand = new Command(() => ExecuteStopButtonCommand(), () => 
            {
                return GettingApiResponses;
            });

            // ensure that stop button is disabled on page creation
            StopButtonCommand.ChangeCanExecute();
        }

        // get the current number of max API pages from API
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
                //statusText.Text = contentString;  // used for debugging
                return;
            }
            MaxApiPages = maxApiPages;
        }

        // event handler that saves item name & id values from API to a local database on saveItemDB button click
        async Task ExecuteSaveItemDBCommand()
        {
            // check internet connection
            ConfigurationStatusText = InternetConnection.CheckForInternetConnection(ConfigurationStatusText);
            if (!InternetConnection.Connection)
            {
                return;
            }

            // set a flag that tells GET request are being sent
            GettingApiResponses = true;
            SaveItemDBCommand.ChangeCanExecute();

            // enable stop button
            StopButtonCommand.ChangeCanExecute();

            // disable clear button
            ClearItemDBCommand.ChangeCanExecute();

            // get number of max API pages
            await GetApiMaxPages();

            // get item names and ids; check if it was successful
            bool getRequestApiResponseSuccess = await GetItemNamesAndIds();
            if (!getRequestApiResponseSuccess) return;

            // change button text
            SaveItemDBButtonText = "Done! Click again to redownload";

            // change flag to signal that GET requests are no longer being sent
            GettingApiResponses = false;
            SaveItemDBCommand.ChangeCanExecute();
            StopButtonCommand.ChangeCanExecute();
            ClearItemDBCommand.ChangeCanExecute();
        }

        // method that asynchronously sends GET requests to the API to receive 200 JSONs per request
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
                await DeserializeAndAddToDatabase(apiResponse);
            }
            return true;
        }

        //  deserializes JSONs and adds ids and names from them to the database
        async Task DeserializeAndAddToDatabase(string apiResponse)
        {
            List<ItemNamesAndIds> itemNamesAndIds = JsonConvert.DeserializeObject<List<ItemNamesAndIds>>(apiResponse);
            await App.Database.SaveItemsAndIdsAsync(itemNamesAndIds);
        }

        // clears local database
        void ExecuteClearItemDBCommand()
        {
            App.Database.ClearItemNamesAndIdsTable();
            ConfigurationStatusText = "Database cleared!";
        }

        // changes GettingApiResponses value to prevent sending GET requests to API
        void ExecuteStopButtonCommand()
        {
            //List<ItemNamesAndIds> test = await App.Database.GetPeopleAsync(); // used for debugging
            //for (int x = 0; x < test.Count; x++)
            //{
            //    ConfigurationStatusText += test[x].id;
            //    ConfigurationStatusText += " " + test[x].name + "\n";
            //}
            GettingApiResponses = false;
            StopButtonCommand.ChangeCanExecute();
            ClearItemDBCommand.ChangeCanExecute();
            SaveItemDBCommand.ChangeCanExecute();
        }
    }
}
