using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Gw2Sharp.Models;
using Gw2Sharp.Models.DTOs;
using System.Threading;

namespace Gw2Sharp.ViewModels
{
    public class ConfigurationViewModel : BaseViewModel
    {
        private bool _gettingApiResponses = false;

        CancellationTokenSource cts = new CancellationTokenSource();

        // stores the number of item API pages
        public int MaxApiPages { get; set; }

        // stores a flag that shows whether the app is currently sending GET requests to the GW2 API
        public bool GettingApiResponses
        {
            get => _gettingApiResponses;
            set
            {
                _gettingApiResponses = value;
                SaveItemDBCommand.ChangeCanExecute();
                StopButtonCommand.ChangeCanExecute();
                ClearItemDBCommand.ChangeCanExecute();
            }
        }
        // binding properties
        public string SaveItemDBButtonText { get; set; } = "Update local item name & id database";
        public string ConfigurationStatusText { get; set; }
        public string ResultsText { get; set; }
        public double PercentageComplete { get; set; } = 0.0;

        // command properties
        public Command SaveItemDBCommand { get; set; }
        public Command ClearItemDBCommand { get; set; }
        public Command StopButtonCommand { get; set; }

        // stores ConfiguartionPage view instance
        public Page CurrentPage { get; set; }

        // constructor
        public ConfigurationViewModel(Page currentPage)
        {
            // assign view instance to CurrentPage property
            CurrentPage = currentPage;

            // create commands with their execute and canExecute methods
            SaveItemDBCommand = new Command(async () => await ExecuteSaveItemDBCommand(), () =>
            {
                return !GettingApiResponses;
            }
            );

            ClearItemDBCommand = new Command(async () => await ExecuteClearItemDBCommand(), () =>
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
                return;
            }
            MaxApiPages = maxApiPages;
        }

        // saves item name & id values from API to a local database on saveItemDB button click
        async Task ExecuteSaveItemDBCommand()
        {
            // create progress instance that will be passed to GetItemNamesAndIdsAsync method later
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();

            // attach ReportProgress event handler to ProgressChanged event
            progress.ProgressChanged += ReportProgress;
            
            // check internet connection
            SaveItemDBButtonText = "Checking internet connection... ";
            ConfigurationStatusText = InternetConnection.CheckForInternetConnection(ConfigurationStatusText);
            if (!InternetConnection.Connection)
            {
                SaveItemDBButtonText = "No internet connection - click again to retry.";
                return;
            }

            // set a flag that tells GET requests are being sent
            GettingApiResponses = true;
            SaveItemDBButtonText = "Preparing for download... ";

            // get number of max API pages
            await GetApiMaxPages();

            try
            {
                // await for GW2 API response results
                var results = await GetItemNamesAndIdsAsync(progress, cts.Token);
                PrintResults(results);
            }
            catch (OperationCanceledException)
            {
                // update UI when GetItemNamesAndIdsAsync is cancelled
                ResultsText += "The async download was cancelled.";
                ConfigurationStatusText = $"Stopped at { PercentageComplete * 100 :0.00}%";
                SaveItemDBButtonText = "Update local item name & id database";
                GettingApiResponses = false;

                // recreate cancellation token source to allow another GetItemNamesAndIdsAsync cancellations
                cts = new CancellationTokenSource();
                return;
            }

            LastApiPageNumber lastApiPage = await App.Database.GetLastDownloadedPageNumber();

            // MaxApiPages + 1 because API pages are zero-based and lastApiPage.ApiPageNumber is not
            if (lastApiPage.ApiPageNumber == MaxApiPages + 1)
            {
                // update UI when last downloaded API page is Max available page
                SaveItemDBButtonText = "Database is up to date - click again to check for updates.";
                ConfigurationStatusText = "Up to date.";
                GettingApiResponses = false;
            }
        }

        // event handler that reports progress of Progress<ProgressReportModel> object
        private void ReportProgress(object sender, ProgressReportModel e)
        {
            PercentageComplete = e.PercentageComplete;
            PrintResults(e.SitesDownloaded);
        }

        // method used for printing URLs of downloaded API pages
        private void PrintResults(List<WebsiteDataModel> results)
        {
            ResultsText = "";
            foreach (var item in results)
            {
                ResultsText += $"Downloaded: { item.WebsiteUrl }.{ Environment.NewLine }";
            }
        }
        
        // method that asynchronously sends GET requests to the API to receive 200 JSONs per request
        async Task<List<WebsiteDataModel>> GetItemNamesAndIdsAsync(IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
        {
            List<string> apiItemLinks = PrepApiItemLinksData();
            List<WebsiteDataModel> apiResponses = new List<WebsiteDataModel>();
            ProgressReportModel report = new ProgressReportModel();
            PercentageComplete = 0.0;

            LastApiPageNumber lastApiPageNumber = await App.Database.GetLastDownloadedPageNumber();
            if (lastApiPageNumber == null)
            {
                lastApiPageNumber = new LastApiPageNumber(0, 1);
            }

            foreach (string link in apiItemLinks.GetRange(lastApiPageNumber.ApiPageNumber, apiItemLinks.Count - lastApiPageNumber.ApiPageNumber))
            {
                WebsiteDataModel apiResponse = new WebsiteDataModel();
                try
                {
                    apiResponse.WebsiteData = await InternetConnection.client.GetStringAsync(link);
                }
                catch (HttpRequestException)
                {
                    ConfigurationStatusText = "Http request error!";
                    break;
                }
                catch (Exception)
                {
                    ConfigurationStatusText = "Unknown exception!";
                    break;
                }
                apiResponse.WebsiteUrl = link;
                cancellationToken.ThrowIfCancellationRequested();

                apiResponses.Add(apiResponse);
                await DeserializeAndAddToDatabase(apiResponse.WebsiteData);
                lastApiPageNumber.ApiPageNumber++;
                await App.Database.SaveLastDownloadedPageNumber(lastApiPageNumber);
                
                // populate progress data model
                report.SitesDownloaded = apiResponses;
                report.PercentageComplete = (double)lastApiPageNumber.ApiPageNumber / apiItemLinks.Count;
                SaveItemDBButtonText = $"Downloading page number: { lastApiPageNumber.ApiPageNumber - 1 } ({  lastApiPageNumber.ApiPageNumber }/{ apiItemLinks.Count })...";
                ConfigurationStatusText = $"{ report.PercentageComplete * 100.0 :0.00}%";

                // update progress
                progress.Report(report);
            }
            return apiResponses;
        }

        // generate API items links and return them as List<string> object
        public List<string> PrepApiItemLinksData()
        {
            List<string> apiItemLinks = new List<string>();

            // prep data
            for (int i = 0; i <= MaxApiPages; ++i)
            {
                apiItemLinks.Add("https://api.guildwars2.com/v2/items?page=" + i + "&page_size=200");
            }

            return apiItemLinks;
        }

        //  deserializes JSONs and adds ids and names from them to the database
        async Task DeserializeAndAddToDatabase(string apiResponse)
        {
            List<ItemNamesAndIds> itemNamesAndIds = JsonConvert.DeserializeObject<List<ItemNamesAndIds>>(apiResponse);
            await App.Database.SaveItemsAndIdsAsync(itemNamesAndIds);
        }

        // clears local database
        async Task ExecuteClearItemDBCommand()
        {
            bool action = await CurrentPage.DisplayAlert("Are you sure?", null, "Yes", "No");
            if (!action)
            {
                return;
            }
            App.Database.ClearItemNamesAndIdsTable();
            ConfigurationStatusText = "Database cleared!";
            ResultsText = "";
            PercentageComplete = 0.0;
        }

        // cancels GetItemNamesAndIdsAsync method execution
        void ExecuteStopButtonCommand()
        {
            cts.Cancel();
        }
    }
}
