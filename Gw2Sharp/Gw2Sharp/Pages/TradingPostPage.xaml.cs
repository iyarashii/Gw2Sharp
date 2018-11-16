using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Gw2Sharp.Schemas;
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

            string itemID;
            string apiResponse;
            //string apiItemLink = "https://api.guildwars2.com/v2/items/" + itemName.Text;
            string gw2spidyLink = "http://www.gw2spidy.com/search/" + itemName.Text;

            try
            {
                itemID = await InternetConnection.client.GetStringAsync(gw2spidyLink);
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

            string startIndex = "data-id=\"";
           // string testStr = itemID;
            itemID = itemID.Substring(itemID.IndexOf(startIndex) + startIndex.Length);
            itemID = itemID.Substring(0, itemID.IndexOf("\""));

            string apiItemLink = "https://api.guildwars2.com/v2/items/" + itemID;

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
            ItemType apiResponseItemType = JsonConvert.DeserializeObject<ItemType>(apiResponse);
            //object apiArmorItemDetails;
            switch (apiResponseItemType.type)
            {
                case "Armor":
                    var apiArmorItemDetails = JsonConvert.DeserializeObject<Armor.RootObject>(apiResponse);
                    ItemDetailsText = "Name: " + apiArmorItemDetails.name;
                    ItemDetailsText += "\nDescription: " + apiArmorItemDetails.description
                     + "\nItem type: " + apiArmorItemDetails.type
                     + "\nLevel: " + apiArmorItemDetails.level + "\n"
                     + "Rarity: " + apiArmorItemDetails.rarity + "\n"
                     + "Vendor value: " + apiArmorItemDetails.vendor_value + "\n"
                     + "Default skin: " + apiArmorItemDetails.default_skin + "\n"
                     + "Chat link: " + apiArmorItemDetails.chat_link + "\n";
                    ItemIconLink = apiArmorItemDetails.icon;
                    break;
                case "Weapon":                    
                    var apiWeaponItemDetails = JsonConvert.DeserializeObject<Weapon.RootObject>(apiResponse);
                    ItemDetailsText = "Name: " + apiWeaponItemDetails.name;
                    ItemDetailsText += "\nDescription: " + apiWeaponItemDetails.description
                    + "\nItem type: " + apiWeaponItemDetails.type
                    + "\nLevel: " + apiWeaponItemDetails.level + "\n"
                    + "Rarity: " + apiWeaponItemDetails.rarity + "\n"
                    + "Vendor value: " + apiWeaponItemDetails.vendor_value + "\n"
                    + "Default skin: " + apiWeaponItemDetails.default_skin + "\n"
                    + "Chat link: " + apiWeaponItemDetails.chat_link + "\n";
                    ItemIconLink = apiWeaponItemDetails.icon;
                    break;
                case "Back":
                    break;
                case "Bag":
                    break;
                case "Consumable":
                    break;
                case "Container":
                    break;
                case "CraftingMaterial":
                    break;
                case "Gathering":
                    break;
                case "Gizmo":
                    break;
                case "MiniPet":
                    break;
                case "Tool":
                    break;
                case "Trinket":
                    break;
                case "Trophy":
                    break;
                case "UpgradeComponent":
                    var apiUpgradeComponentItemDetails = JsonConvert.DeserializeObject<UpgradeComponent.RootObject>(apiResponse);
                    ItemDetailsText = "Name: " + apiUpgradeComponentItemDetails.name;
                    ItemDetailsText += "\nDescription: " + apiUpgradeComponentItemDetails.description;
                    ItemDetailsText += "\nItem type: " + apiUpgradeComponentItemDetails.type;
                    ItemDetailsText += "\nLevel: " + apiUpgradeComponentItemDetails.level + "\n";
                    ItemDetailsText += "Rarity: " + apiUpgradeComponentItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiUpgradeComponentItemDetails.vendor_value + "\n";
                    ItemDetailsText += "Chat link: " + apiUpgradeComponentItemDetails.chat_link + "\n";
                    ItemIconLink = apiUpgradeComponentItemDetails.icon;
                    break;
                default:
                    BindingContext = this;
                    return;
            }         
            responseTextLayout.IsVisible = true;
            BindingContext = this;
        }

    }
}