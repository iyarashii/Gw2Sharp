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
using Xamarin.Essentials;

namespace Gw2Sharp.ViewModels
{
    public class TradingPostViewModel : BaseViewModel
    {
        // properties used for bindings
        public string ItemDetailsText { get; set; }
        public string ItemPriceText { get; set; }
        public string ItemSellsPriceText { get; set; }
        public string ItemIconLink { get; set; }
        public string ItemID { get; set; }
        public string ItemNameEntryText { get; set; }
        public string TradingPostStatusText { get; set; }
        public bool IsTradingPostStatusTextVisible { get; set; }
        public bool IsItemPriceLayoutVisible { get; set; }
        public string BuysGoldText { get; set; }
        public string BuysSilverText { get; set; }
        public string BuysCopperText { get; set; }
        public string SellsGoldText { get; set; }
        public string SellsSilverText { get; set; }
        public string SellsCopperText { get; set; }
        public bool IsResponseTextLayoutVisible { get; set; }
        public bool IsBuysGoldCoinImageVisible { get; set; }
        public bool IsBuysSilverCoinImageVisible { get; set; }
        public bool IsSellsGoldCoinImageVisible { get; set; }
        public bool IsSellsSilverCoinImageVisible { get; set; }


        public Command ShowItemPriceCommand { get; set; }
        public Command ShowItemCommand { get; set; }
        public Command CopyChatLinkCommand { get; set; }

        // constructor
        public TradingPostViewModel()
        {
            IsTradingPostStatusTextVisible = false;
            IsItemPriceLayoutVisible = false;
            IsResponseTextLayoutVisible = false;
            IsBuysGoldCoinImageVisible = false;
            IsBuysSilverCoinImageVisible = false;
            IsSellsGoldCoinImageVisible = false;
            IsSellsSilverCoinImageVisible = false;

            ShowItemPriceCommand = new Command(async () => await ExecuteShowItemPriceCommand());
            ShowItemCommand = new Command(async () => await ExecuteShowItemCommand());
            CopyChatLinkCommand = new Command(async () => await ExecuteCopyChatLinkCommand());
        }

        // method used for checking whether typed in item name exists in local database
        async Task<bool> CheckLocalItemDB()
        {
            //List<ItemNamesAndIds> returnedItem = await App.Database.GetItemAsync(ItemNameEntryText);
            ItemNamesAndIds returnedItem = await App.Database.GetItemAsync(ItemNameEntryText);
            //if (returnedItem.Count == 0)
            if (returnedItem == null)
            {
                TradingPostStatusText = "Item name not found in local item database!";
                IsTradingPostStatusTextVisible = true;
                return false;
            }
            //ItemID = returnedItem[0].id.ToString();
            ItemID = returnedItem.id.ToString();
            return true;
        }

        // method used to display typed in item price and quantity
        async Task ExecuteShowItemPriceCommand()
        {
            string apiResponse;
            IsItemPriceLayoutVisible = false;
            IsBuysGoldCoinImageVisible = false;
            IsBuysSilverCoinImageVisible = false;
            IsSellsGoldCoinImageVisible = false;
            IsSellsSilverCoinImageVisible = false;

            // check internet connection
            TradingPostStatusText = InternetConnection.CheckForInternetConnection(TradingPostStatusText);
            if (!InternetConnection.connection)
            {
                IsTradingPostStatusTextVisible = true;
                return;
            }

            if (! await CheckLocalItemDB()) return;

            string apiItemLink = "https://api.guildwars2.com/v2/commerce/prices/" + ItemID;

            try
            {
                apiResponse = await InternetConnection.client.GetStringAsync(apiItemLink);
            }
            catch (HttpRequestException)
            {
                TradingPostStatusText = "Wrong name or item can't be put on trading post.";
                IsTradingPostStatusTextVisible = true;
                return;
            }
            catch (Exception)
            {
                TradingPostStatusText = "Unknown exception!";
                IsTradingPostStatusTextVisible = true;
                return;
            }

            var apiItemPriceResponse = JsonConvert.DeserializeObject<ItemTpPrice.RootObject>(apiResponse);
            string copperUnitPrice, silverUnitPrice = null, goldUnitPrice = null;
            // BUYS
            if (apiItemPriceResponse.buys.unit_price.ToString().Length >= 2)
            {
                copperUnitPrice = apiItemPriceResponse.buys.unit_price.ToString().Substring(apiItemPriceResponse.buys.unit_price.ToString().Length - 2);
                if (apiItemPriceResponse.buys.unit_price.ToString().Length >= 3)
                {
                    if (apiItemPriceResponse.buys.unit_price.ToString().Length == 3)
                        silverUnitPrice = apiItemPriceResponse.buys.unit_price.ToString().Substring(apiItemPriceResponse.buys.unit_price.ToString().Length - 3, 1);
                    else silverUnitPrice = apiItemPriceResponse.buys.unit_price.ToString().Substring(apiItemPriceResponse.buys.unit_price.ToString().Length - 4, 2);
                    IsBuysSilverCoinImageVisible = true;
                    if (apiItemPriceResponse.buys.unit_price.ToString().Length > 4)
                    {
                        goldUnitPrice = apiItemPriceResponse.buys.unit_price.ToString().Substring(0, apiItemPriceResponse.buys.unit_price.ToString().Length - 4);
                        IsBuysGoldCoinImageVisible = true;
                    }
                }
            }
            else copperUnitPrice = apiItemPriceResponse.buys.unit_price.ToString();

            BuysGoldText = goldUnitPrice;
            BuysSilverText = silverUnitPrice;
            BuysCopperText = copperUnitPrice;

            // SELLS
            if (apiItemPriceResponse.sells.unit_price.ToString().Length >= 2)
            {
                copperUnitPrice = apiItemPriceResponse.sells.unit_price.ToString().Substring(apiItemPriceResponse.sells.unit_price.ToString().Length - 2);
                if (apiItemPriceResponse.sells.unit_price.ToString().Length >= 3)
                {
                    if (apiItemPriceResponse.sells.unit_price.ToString().Length == 3)
                        silverUnitPrice = apiItemPriceResponse.sells.unit_price.ToString().Substring(apiItemPriceResponse.sells.unit_price.ToString().Length - 3, 1);
                    else silverUnitPrice = apiItemPriceResponse.sells.unit_price.ToString().Substring(apiItemPriceResponse.sells.unit_price.ToString().Length - 4, 2);
                    IsSellsSilverCoinImageVisible = true;
                    if (apiItemPriceResponse.sells.unit_price.ToString().Length > 4)
                    {
                        goldUnitPrice = apiItemPriceResponse.sells.unit_price.ToString().Substring(0, apiItemPriceResponse.sells.unit_price.ToString().Length - 4);
                        IsSellsGoldCoinImageVisible = true;
                    }
                }
            }
            else copperUnitPrice = apiItemPriceResponse.sells.unit_price.ToString();

            ItemPriceText = "Buy orders:\n" + "Quantity: " + apiItemPriceResponse.buys.quantity;
            ItemSellsPriceText = "Sell orders:\n" + "Quantity: " + apiItemPriceResponse.sells.quantity;
            SellsGoldText = goldUnitPrice;
            SellsSilverText = silverUnitPrice;
            SellsCopperText = copperUnitPrice;

            IsItemPriceLayoutVisible = true;
        }

        async Task ExecuteShowItemCommand()
        {
            IsResponseTextLayoutVisible = false;
            IsTradingPostStatusTextVisible = false;
            string apiResponse;

            // check internet connection
            TradingPostStatusText = InternetConnection.CheckForInternetConnection(TradingPostStatusText);
            if (!InternetConnection.connection)
            {
                IsTradingPostStatusTextVisible = true;
                return;
            }

            if (!await CheckLocalItemDB()) return;

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
                TradingPostStatusText = "No such id";
                IsTradingPostStatusTextVisible = true;
                return;
            }
            catch (Exception)
            {
                TradingPostStatusText = "Unknown exception!";
                IsTradingPostStatusTextVisible = true;
                return;
            }

            ItemType apiResponseItemType;

            try
            {
                apiResponseItemType = JsonConvert.DeserializeObject<ItemType>(apiResponse);
            }
            catch (JsonSerializationException)
            {
                TradingPostStatusText = "JsonSerialization exception!";
                IsTradingPostStatusTextVisible = true;
                return;
            }

            IsResponseTextLayoutVisible = true;

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
                    //+ "Vendor value: " + apiWeaponItemDetails.vendor_value + "\n"
                    + "Default skin: " + apiWeaponItemDetails.default_skin + "\n"/* + apiWeaponItemDetails.icon + " test\n"*/
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
            IsResponseTextLayoutVisible = true;
        }

        // method used for copying item chatlink to clipboard via button press
        async Task ExecuteCopyChatLinkCommand()
        {
            try
            {
                await Clipboard.SetTextAsync(ItemDetailsText.Substring(ItemDetailsText.IndexOf("["), (ItemDetailsText.IndexOf("]")) - ItemDetailsText.IndexOf(" [")));
            }
            catch
            {
                TradingPostStatusText = "Chat link not available!";
                IsTradingPostStatusTextVisible = true;
                return;
            }
            TradingPostStatusText = "Copied!";
            IsTradingPostStatusTextVisible = true;
        }
    }
}
