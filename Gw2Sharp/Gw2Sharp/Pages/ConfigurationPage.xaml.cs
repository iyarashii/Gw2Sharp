﻿using System;
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

namespace Gw2Sharp.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ConfigurationPage : ContentPage
	{
        public int MaxApiPages { get; set; }
		public ConfigurationPage()
		{
			InitializeComponent();
		}
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
            contentString = contentString.Substring(contentString.IndexOf("-") + 2, contentString.IndexOf(".\"") - contentString.IndexOf("-") - 2);
            if (!int.TryParse(contentString, out int maxApiPages))
            {
                statusText.Text = "Error occured while parsing to int!";
                //statusText.Text = contentString;
                BindingContext = this;
                return;
            }
            MaxApiPages = maxApiPages;
        }
        async void OnSaveItemDB(object sender, EventArgs e)
        {
            if (!MainPage.Connection.CheckForInternetConnection(statusText)) return;
            GetApiMaxPages();
            string itemDatabase = null;
            string apiResponse = null;
            int i = 0;
            string apiItemLink = "https://api.guildwars2.com/v2/items?page=" + i + "&page_size=200";
            
            for (i = 0; i <= MaxApiPages; ++i)
            {
                apiItemLink = "https://api.guildwars2.com/v2/items?page=" + i + "&page_size=200";
                try
                {
                    apiResponse = await InternetConnection.client.GetStringAsync(apiItemLink);
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
                saveItemDB.Text = "Getting api responses in progress... " + "(" + i + "/" + MaxApiPages + ")";
                BindingContext = this;
                List<ItemNamesAndIds> itemNamesAndIds = JsonConvert.DeserializeObject<List<ItemNamesAndIds>>(apiResponse);
                for (int x = 0; x < itemNamesAndIds.Count; x++)
                {
                    itemDatabase += itemNamesAndIds[x].id;
                    itemDatabase += " " + itemNamesAndIds[x].name + "\n";
                }
                File.AppendAllText(TradingPostPage.ItemDBPath, itemDatabase);
                itemDatabase = null;
            }
            saveItemDB.Text = "Done! Click again to redownload and overwrite local database file";
            BindingContext = this;
        }
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
        
    }
}