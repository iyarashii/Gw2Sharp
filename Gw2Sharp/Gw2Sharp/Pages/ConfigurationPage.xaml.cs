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

namespace Gw2Sharp.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ConfigurationPage : ContentPage
	{
		public ConfigurationPage()
		{
			InitializeComponent();
		}
     
    async void OnSaveItemDB(object sender, EventArgs e)
        {
            if (!MainPage.Connection.CheckForInternetConnection(statusText)) return;

            string itemDatabase = null;
            string apiResponse = null;
            int i = 0;
            string apiItemLink = "https://api.guildwars2.com/v2/items?page=" + i + "&page_size=200";
            
            for (i = 0; i <= 2; ++i)
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
                saveItemDB.Text = "Getting api responses in progress... " + "(" + i + "/" + "269)";
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

            //File.WriteAllText(TradingPostPage.ItemDBPath, itemDatabase);
            saveItemDB.Text = "Done! Click again to redownload and overwrite local database file";
            BindingContext = this;
        }
        void OnDeleteItemDB()
        {
            
            try
            {
                if (File.Exists(TradingPostPage.ItemDBPath))
                MainPage.Connection.GrantAccess(TradingPostPage.ItemDBPath);
            }
            catch (System.IO.FileNotFoundException)
            {
                statusText.Text = "Database not found!";
                BindingContext = this;
                return;
            }

            try
            {
                File.Delete(TradingPostPage.ItemDBPath);
            }
            catch
            {
                statusText.Text = "Delete";
                BindingContext = this;
                return;
            }
            statusText.Text = "Database file successfully deleted!";
            BindingContext = this;
        }
        
    }
}