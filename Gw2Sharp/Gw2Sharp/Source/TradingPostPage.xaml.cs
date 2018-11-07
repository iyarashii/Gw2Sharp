using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Gw2Sharp.Source;
using System.Net.Http;
using Newtonsoft.Json;

namespace Gw2Sharp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TradingPostPage : ContentPage
	{
        public string ItemDetailsText { get; set; }
        public string ItemIconLink { get; set; }
        public TradingPostPage ()
		{
			InitializeComponent ();
		}
        async void OnShowItem(object sender, EventArgs e)
        {
            BindingContext = null;
            responseTextLayout.IsVisible = false;
            iconText.IsVisible = false;
            iconText.Text = "Icon:";

            if (!InternetConnection.CheckForInternetConnection())
            {
                iconText.Text = "No internet connection!";
                iconText.IsVisible = true;
                BindingContext = this;
                return;
            }

            string apiResponse;
            string apiItemLink = "https://api.guildwars2.com/v2/items/" + itemName.Text;

            try
            {
                apiResponse = await InternetConnection.client.GetStringAsync(apiItemLink);
            }
            catch (HttpRequestException)
            {
                iconText.Text = "No such id";
                BindingContext = this;
                return;
            }
            catch (Exception)
            {
                iconText.Text = "Unknown exception!";
                BindingContext = this;
                return;
            }
            finally
            {
                iconText.IsVisible = true;
            }
            Weapon.RootObject apiItemDetails = JsonConvert.DeserializeObject<Weapon.RootObject>(apiResponse);
            ItemDetailsText = "Name: " + apiItemDetails.name;
            ItemDetailsText += "\ndescription:" + apiItemDetails.description;
            ItemIconLink = apiItemDetails.icon;            
            responseTextLayout.IsVisible = true;
            BindingContext = this;
        }

    }
}