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

namespace Gw2Sharp.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TradingPostPage : ContentPage
	{
        public string ItemDetailsText { get; set; }
        public string ItemPriceText { get; set; }
        public string ItemSellsPriceText { get; set; }
        public string ItemIconLink { get; set; }
        public string ItemID { get; set; }
        //static InternetConnection Connection = new InternetConnection();

        public double SellsGoldTextFontSize { get { return enterItemNameText.FontSize; } }
        static public string ItemDBPath { get  { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "itemDB.txt"); } }
        public TradingPostPage ()
		{
			InitializeComponent ();
		}

        bool CheckLocalItemDB()
        {
            string lineItemName = null;
            foreach (string line in File.ReadLines(ItemDBPath))
            {
                lineItemName = line.Substring(line.IndexOf(" "));
                if ((line.IndexOf(itemName.Text, StringComparison.InvariantCultureIgnoreCase) != -1) && (lineItemName.Length - 1 == itemName.Text.Length) && itemName.Text != "")
                {
                    ItemID = line.Substring(0, line.IndexOf(" "));
                    break;
                }
            }
            if (ItemID == null)
            {
                statusText.Text = "Item name not found in local item database!";
                BindingContext = this;
                statusText.IsVisible = true;
                return false;
            }
            return true;
        }
        async void OnShowItemPrice(object sender, EventArgs e)
        {
            
            BindingContext = null;
            string apiResponse;
            itemPriceLayout.IsVisible = false;
            buysGoldCoinImage.IsVisible = false;
            buysSilverCoinImage.IsVisible = false;
            sellsSilverCoinImage.IsVisible = false;
            sellsGoldCoinImage.IsVisible = false;

            if (!MainPage.Connection.CheckForInternetConnection(statusText)) return;

            if (!CheckLocalItemDB()) return;

            string apiItemLink = "https://api.guildwars2.com/v2/commerce/prices/" + ItemID;

            try
            {
                apiResponse = await InternetConnection.client.GetStringAsync(apiItemLink);
            }
            catch (HttpRequestException)
            {
                statusText.Text = "Wrong name or item can't be put on trading post.";
                statusText.IsVisible = true;
                BindingContext = this;
                return;
            }
            catch (Exception)
            {
                statusText.Text = "Unknown exception!";
                statusText.IsVisible = true;
                BindingContext = this;
                return;
            }
            var apiItemPriceResponse = JsonConvert.DeserializeObject<ItemTpPrice.RootObject>(apiResponse);
            string copperUnitPrice = null, silverUnitPrice = null, goldUnitPrice = null;
            // BUYS
            if (apiItemPriceResponse.buys.unit_price.ToString().Length >= 2)
            {
                copperUnitPrice = apiItemPriceResponse.buys.unit_price.ToString().Substring(apiItemPriceResponse.buys.unit_price.ToString().Length - 2);
                if (apiItemPriceResponse.buys.unit_price.ToString().Length >= 3)
                {
                    if(apiItemPriceResponse.buys.unit_price.ToString().Length == 3)
                        silverUnitPrice = apiItemPriceResponse.buys.unit_price.ToString().Substring(apiItemPriceResponse.buys.unit_price.ToString().Length - 3, 1);
                    else silverUnitPrice = apiItemPriceResponse.buys.unit_price.ToString().Substring(apiItemPriceResponse.buys.unit_price.ToString().Length - 4, 2);
                    buysSilverCoinImage.IsVisible = true;
                    if (apiItemPriceResponse.buys.unit_price.ToString().Length > 4)
                    {
                        goldUnitPrice = apiItemPriceResponse.buys.unit_price.ToString().Substring(0, apiItemPriceResponse.buys.unit_price.ToString().Length - 4);
                        buysGoldCoinImage.IsVisible = true;
                    }
                }
            } 
            else copperUnitPrice = apiItemPriceResponse.buys.unit_price.ToString();
            buysGoldText.Text = goldUnitPrice;
            buysSilverText.Text = silverUnitPrice;
            buysCopperText.Text = copperUnitPrice;


            // SELLS
            if (apiItemPriceResponse.sells.unit_price.ToString().Length >= 2)
            {
                copperUnitPrice = apiItemPriceResponse.sells.unit_price.ToString().Substring(apiItemPriceResponse.sells.unit_price.ToString().Length - 2);
                if (apiItemPriceResponse.sells.unit_price.ToString().Length >= 3)
                {
                    if (apiItemPriceResponse.sells.unit_price.ToString().Length == 3)
                        silverUnitPrice = apiItemPriceResponse.sells.unit_price.ToString().Substring(apiItemPriceResponse.sells.unit_price.ToString().Length - 3, 1);
                    else silverUnitPrice = apiItemPriceResponse.sells.unit_price.ToString().Substring(apiItemPriceResponse.sells.unit_price.ToString().Length - 4, 2);
                    sellsSilverCoinImage.IsVisible = true;
                    if (apiItemPriceResponse.sells.unit_price.ToString().Length > 4)
                    {
                        goldUnitPrice = apiItemPriceResponse.sells.unit_price.ToString().Substring(0, apiItemPriceResponse.sells.unit_price.ToString().Length - 4);
                        sellsGoldCoinImage.IsVisible = true;
                    }
                }
            }
            else copperUnitPrice = apiItemPriceResponse.sells.unit_price.ToString();

            ItemPriceText = "Buy orders:\n" + "Quantity: " + apiItemPriceResponse.buys.quantity;
            ItemSellsPriceText = "Sell orders:\n" + "Quantity: " + apiItemPriceResponse.sells.quantity;
            sellsGoldText.Text = goldUnitPrice;
            sellsSilverText.Text = silverUnitPrice;
            sellsCopperText.Text = copperUnitPrice;

            BindingContext = this;
            itemPriceLayout.IsVisible = true;
        }
        //async void OnSaveItemDB(object sender, EventArgs e)
        //{
        //    if (!CheckForInternetConnection()) return;

        //    string itemDatabase = null;
        //    string apiResponse = null;    
        //    int i = 0;
        //    string apiItemLink = "https://api.guildwars2.com/v2/items?page=" + i + "&page_size=200";

        //    for (i = 0; i <= 269; ++i)
        //    {
        //        apiItemLink = "https://api.guildwars2.com/v2/items?page=" + i + "&page_size=200";
        //        try
        //        {
        //            apiResponse = await InternetConnection.client.GetStringAsync(apiItemLink);
        //        }
        //        catch (HttpRequestException)
        //        {
        //            statusText.Text = "Http request error!";
        //            BindingContext = this;
        //            return;
        //        }
        //        catch (Exception)
        //        {
        //            statusText.Text = "Unknown exception!";
        //            BindingContext = this;
        //            return;
        //        }
        //        saveItemDB.Text = "Getting api responses in progress... " + "(" + i + "/" + "269)";
        //        BindingContext = this;
        //        List<ItemNamesAndIds> itemNamesAndIds = JsonConvert.DeserializeObject<List<ItemNamesAndIds>>(apiResponse);
        //        for (int x = 0; x < itemNamesAndIds.Count; x++)
        //        {
        //            itemDatabase += itemNamesAndIds[x].id;
        //            itemDatabase += " " + itemNamesAndIds[x].name + "\n";
        //        }
        //    }
          
        //    File.WriteAllText(ItemDBPath, itemDatabase);
        //    saveItemDB.Text = "Done! Click again to redownload and overwrite local database file";
        //    BindingContext = this;
        //}

        async void OnShowItem(object sender, EventArgs e)
        {
            BindingContext = null;
            responseTextLayout.IsVisible = false;
            statusText.IsVisible = false;
            string apiResponse;
           
            if(!MainPage.Connection.CheckForInternetConnection(statusText)) return;

            if(!CheckLocalItemDB()) return;
            
            //string gw2spidyLink = "http://www.gw2spidy.com/search/" + itemName.Text;          
            //string startIndex = "data-id=\"";
            //itemID = itemID.Substring(itemID.IndexOf(startIndex) + startIndex.Length);
            //itemID = itemID.Substring(0, itemID.IndexOf("\""));

            string apiItemLink = "https://api.guildwars2.com/v2/items/" + ItemID;

            try
            {
                apiResponse = await InternetConnection.client.GetStringAsync(apiItemLink);
            }
            catch (HttpRequestException)
            {
                statusText.Text = "No such id";
                statusText.IsVisible = true;
                BindingContext = this;
                return;
            }
            catch (Exception)
            {
                statusText.Text = "Unknown exception!";
                statusText.IsVisible = true;
                BindingContext = this;
                return;
            }

            ItemType apiResponseItemType;
            try
            {
                apiResponseItemType = JsonConvert.DeserializeObject<ItemType>(apiResponse);
            }
            catch (JsonSerializationException)
            {
                statusText.Text = "JsonSerialization exception!";
                BindingContext = this;
                statusText.IsVisible = true;
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
                    return;
            }
            responseTextLayout.IsVisible = true;
            BindingContext = this;
        }

    }
}