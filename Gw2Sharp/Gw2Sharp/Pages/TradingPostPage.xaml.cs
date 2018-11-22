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
using System.IO;

namespace Gw2Sharp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TradingPostPage : ContentPage
	{
        public string ItemDetailsText { get; set; }
        public string ItemIconLink { get; set; }
        public string ItemDBPath { get  { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "itemDB.txt"); } }
        public TradingPostPage ()
		{
			InitializeComponent ();
		}
        void OnItemTradingPostPrice(object sender, EventArgs e)
        {
            BindingContext = null;

        }
        async void OnSaveItem(object sender, EventArgs e)
        {
            //BindingContext = null;
            //responseTextLayout.IsVisible = false;
            //iconText.IsVisible = false;
            //iconText.Text = "";

            if (!InternetConnection.CheckForInternetConnection())
            {
                iconText.Text = "No internet connection!";
                iconText.IsVisible = true;
                BindingContext = this;
                return;
            }

            string itemDatabase = null;
            string apiResponse = null;    
            int i = 0;
            string apiItemLink = "https://api.guildwars2.com/v2/items?page=" + i + "&page_size=200";

            for (i = 0; i <= 269; ++i)
            {
                apiItemLink = "https://api.guildwars2.com/v2/items?page=" + i + "&page_size=200";
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

                List<ItemNamesAndIds> itemNamesAndIds = JsonConvert.DeserializeObject<List<ItemNamesAndIds>>(apiResponse);
                for (int x = 0; x < itemNamesAndIds.Count; x++)
                {
                    itemDatabase += itemNamesAndIds[x].id;
                    itemDatabase += " " + itemNamesAndIds[x].name + "\n";
                }
            }
          
            File.WriteAllText(ItemDBPath, itemDatabase);
        }

        async void OnShowItem(object sender, EventArgs e)
        {
            BindingContext = null;
            responseTextLayout.IsVisible = false;
            iconText.IsVisible = false;
            iconText.Text = "Icon:";
            string itemID = null;
            string apiResponse;
            string itemLineName = null;

            if (!InternetConnection.CheckForInternetConnection())
            {
                iconText.Text = "No internet connection!";
                iconText.IsVisible = true;
                BindingContext = this;
                return;
            }
            
            foreach (string line in File.ReadLines(ItemDBPath))
            {
                itemLineName = line.Substring(line.IndexOf(" "));
                if ( (line.IndexOf(itemName.Text, StringComparison.InvariantCultureIgnoreCase) != -1 ) && ( itemLineName.Length - 1 == itemName.Text.Length )) 
                {
                    itemID = line.Substring(0, line.IndexOf(" "));
                    break;
                }
            }
            if (itemID == null)
            {
                iconText.Text = "Item name not found in local item database!";
                BindingContext = this;
                iconText.IsVisible = true;
                return;
            }
            
            //string gw2spidyLink = "http://www.gw2spidy.com/search/" + itemName.Text;          
            //string startIndex = "data-id=\"";
            //itemID = itemID.Substring(itemID.IndexOf(startIndex) + startIndex.Length);
            //itemID = itemID.Substring(0, itemID.IndexOf("\""));

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
            ItemType apiResponseItemType;
            try
            {
                apiResponseItemType = JsonConvert.DeserializeObject<ItemType>(apiResponse);
            }
            catch (JsonSerializationException)
            {
                iconText.Text = "JsonSerialization exception!";
                BindingContext = this;
                iconText.IsVisible = true;
                return;
            }

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
                    var apiBackItemDetails = JsonConvert.DeserializeObject<BackItem.RootObject>(apiResponse);
                    ItemDetailsText = "Name: " + apiBackItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiBackItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiBackItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiBackItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiBackItemDetails.vendor_value + "\n";
                    ItemIconLink = apiBackItemDetails.icon;
                    break;
                case "Bag":
                    var apiBagItemDetails = JsonConvert.DeserializeObject<Bag.RootObject>(apiResponse);
                    ItemDetailsText = "Name: " + apiBagItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiBagItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiBagItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiBagItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiBagItemDetails.vendor_value + "\n";
                    ItemIconLink = apiBagItemDetails.icon;
                    break;
                case "Consumable":
                    var apiConsumableItemDetails = JsonConvert.DeserializeObject<Consumable.RootObject>(apiResponse);
                    ItemDetailsText = "Name: " + apiConsumableItemDetails.name + "\n";
                    ItemDetailsText += "Item type: " + apiConsumableItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiConsumableItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiConsumableItemDetails.vendor_value + "\n";
                    ItemIconLink = apiConsumableItemDetails.icon;
                    break;
                case "Container":
                    var apiContainerItemTypeDetails = JsonConvert.DeserializeObject<ContainerItemType.RootObject>(apiResponse);
                    ItemDetailsText = "Name: " + apiContainerItemTypeDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiContainerItemTypeDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiContainerItemTypeDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiContainerItemTypeDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiContainerItemTypeDetails.vendor_value + "\n";
                    ItemIconLink = apiContainerItemTypeDetails.icon;
                    break;
                case "CraftingMaterial":
                    var apiCraftingMaterialItemDetails = JsonConvert.DeserializeObject<CraftingMaterial.RootObject>(apiResponse);
                    ItemDetailsText = "Name: " + apiCraftingMaterialItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiCraftingMaterialItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiCraftingMaterialItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiCraftingMaterialItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiCraftingMaterialItemDetails.vendor_value + "\n";
                    ItemIconLink = apiCraftingMaterialItemDetails.icon;
                    break;
                case "Gathering":
                    var apiGatheringItemDetails = JsonConvert.DeserializeObject<Gathering.RootObject>(apiResponse);
                    ItemDetailsText = "Name: " + apiGatheringItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiGatheringItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiGatheringItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiGatheringItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiGatheringItemDetails.vendor_value + "\n";
                    ItemIconLink = apiGatheringItemDetails.icon;
                    break;
                case "Gizmo":
                    var apiGizmoItemDetails = JsonConvert.DeserializeObject<Gizmo.RootObject>(apiResponse);
                    ItemDetailsText = "Name: " + apiGizmoItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiGizmoItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiGizmoItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiGizmoItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiGizmoItemDetails.vendor_value + "\n";
                    ItemIconLink = apiGizmoItemDetails.icon;
                    break;
                case "MiniPet":
                    var apiMiniPetItemDetails = JsonConvert.DeserializeObject<MiniPet.RootObject>(apiResponse);
                    ItemDetailsText = "Name: " + apiMiniPetItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiMiniPetItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiMiniPetItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiMiniPetItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiMiniPetItemDetails.vendor_value + "\n";
                    ItemIconLink = apiMiniPetItemDetails.icon;
                    break;
                case "Tool":
                    var apiToolItemDetails = JsonConvert.DeserializeObject<Tool.RootObject>(apiResponse);
                    ItemDetailsText = "Name: " + apiToolItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiToolItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiToolItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiToolItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiToolItemDetails.vendor_value + "\n";
                    ItemIconLink = apiToolItemDetails.icon;
                    break;
                case "Trinket":
                    var apiTrinketItemDetails = JsonConvert.DeserializeObject<Trinket.RootObject>(apiResponse);
                    ItemDetailsText = "Name: " + apiTrinketItemDetails.name + "\n";
                    ItemDetailsText += "Item type: " + apiTrinketItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiTrinketItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiTrinketItemDetails.vendor_value + "\n";
                    ItemIconLink = apiTrinketItemDetails.icon;
                    break;
                case "Trophy":
                    var apiTrophyItemDetails = JsonConvert.DeserializeObject<Trophy.RootObject>(apiResponse);
                    ItemDetailsText = "Name: " + apiTrophyItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiTrophyItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiTrophyItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiTrophyItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiTrophyItemDetails.vendor_value + "\n";
                    ItemIconLink = apiTrophyItemDetails.icon;
                    break;
                case "UpgradeComponent":
                    var apiUpgradeComponentItemDetails = JsonConvert.DeserializeObject<UpgradeComponent.RootObject>(apiResponse);
                    ItemDetailsText = "Name: " + apiUpgradeComponentItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiUpgradeComponentItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiUpgradeComponentItemDetails.type + "\n";
                    ItemDetailsText += "Level: " + apiUpgradeComponentItemDetails.level + "\n";
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