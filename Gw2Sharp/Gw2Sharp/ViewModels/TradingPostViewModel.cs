// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Gw2Sharp/blob/master/LICENSE for license details.

using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Gw2Sharp.Models.DTOs;
using Gw2Sharp.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Gw2Sharp.ViewModels
{
    public class TradingPostViewModel : BaseViewModel
    {
        // properties used for bindings
        #region BindingProperties
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
        #endregion

        // command properties
        public Command ShowItemPriceCommand { get; set; }
        public Command ShowItemCommand { get; set; }
        public Command CopyChatLinkCommand { get; set; }

        // stores api response value
        public string ApiResponse { get; set; }

        // stores item type from api response
        ItemType ApiResponseItemType { get; set; }

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

        // checks whether typed in item name exists in the local database
        async Task<bool> CheckLocalItemDB()
        {
            ItemNamesAndIds returnedItem = await App.Database.GetItemAsync(ItemNameEntryText);

            if (returnedItem == null)
            {
                TradingPostStatusText = "Item name not found in local item database!";
                IsTradingPostStatusTextVisible = true;
                return false;
            }

            ItemID = returnedItem.id.ToString();
            return true;
        }

        // displays price and quantity of typed in item
        async Task ExecuteShowItemPriceCommand()
        {
            string apiResponse;

            HideCoinImages();

            // check internet connection
            TradingPostStatusText = InternetConnection.CheckForInternetConnection(TradingPostStatusText);
            if (!InternetConnection.Connection)
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

            // display item BUYS listings price 
            DisplayItemPrice(apiItemPriceResponse.buys.unit_price, true);

            // display item SELLS listings price
            DisplayItemPrice(apiItemPriceResponse.sells.unit_price, false);

            ItemPriceText = "Buy orders:\n" + "Quantity: " + apiItemPriceResponse.buys.quantity;
            ItemSellsPriceText = "Sell orders:\n" + "Quantity: " + apiItemPriceResponse.sells.quantity;

            IsItemPriceLayoutVisible = true;
        }

        // displays item price broken down into gold, silver and copper coins
        void DisplayItemPrice(int itemUnitPrice, bool buys)
        {
            int? copperUnitPrice, silverUnitPrice = null, goldUnitPrice = null;

            copperUnitPrice = itemUnitPrice / 10 % 10 * 10 + itemUnitPrice % 10;
            if(itemUnitPrice > 99)
            {
                silverUnitPrice = itemUnitPrice / 1000 % 10 * 10 + itemUnitPrice / 100 % 10;

                if (buys)
                {
                    IsBuysSilverCoinImageVisible = true;
                }
                else
                {
                    IsSellsSilverCoinImageVisible = true;
                }
            }
            if (itemUnitPrice > 9999)
            {
                goldUnitPrice = itemUnitPrice / 10_000;

                if (buys)
                {
                    IsBuysGoldCoinImageVisible = true;
                }
                else
                {
                    IsSellsGoldCoinImageVisible = true;
                }

            }

            if (buys)
            {
                BuysGoldText = goldUnitPrice.ToString();
                BuysSilverText = silverUnitPrice.ToString();
                BuysCopperText = copperUnitPrice.ToString();
            }
            else
            {
                SellsGoldText = goldUnitPrice.ToString();
                SellsSilverText = silverUnitPrice.ToString();
                SellsCopperText = copperUnitPrice.ToString();
            }
        }

        // hides coin images
        void HideCoinImages()
        {
            IsItemPriceLayoutVisible = false;
            IsBuysGoldCoinImageVisible = false;
            IsBuysSilverCoinImageVisible = false;
            IsSellsGoldCoinImageVisible = false;
            IsSellsSilverCoinImageVisible = false;
        }

        // displays item details
        async Task ExecuteShowItemCommand()
        {
            IsResponseTextLayoutVisible = false;
            IsTradingPostStatusTextVisible = false;
            ItemIconLink = null;

            // check internet connection
            TradingPostStatusText = InternetConnection.CheckForInternetConnection(TradingPostStatusText);
            if (!InternetConnection.Connection)
            {
                IsTradingPostStatusTextVisible = true;
                return;
            }

            if (!await CheckLocalItemDB()) return;

            string apiItemLink = "https://api.guildwars2.com/v2/items/" + ItemID;

            if (!await GetApiItemDetailsResponse(apiItemLink)) return;

            if (!GetItemType()) return;

            switch (ApiResponseItemType.type)
            {
                case "Armor":
                    var apiArmorItemDetails = JsonConvert.DeserializeObject<Armor.RootObject>(ApiResponse);
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
                    var apiWeaponItemDetails = JsonConvert.DeserializeObject<Weapon.RootObject>(ApiResponse);
                    ItemDetailsText = "Name: " + apiWeaponItemDetails.name;
                    ItemDetailsText += "\nDescription: " + apiWeaponItemDetails.description
                    + "\nItem type: " + apiWeaponItemDetails.type
                    + "\nLevel: " + apiWeaponItemDetails.level + "\n"
                    + "Rarity: " + apiWeaponItemDetails.rarity + "\n"
                    //+ "Vendor value: " + apiWeaponItemDetails.vendor_value + "\n"
                    + "Default skin: " + apiWeaponItemDetails.default_skin + "\n"
                    + "Chat link: " + apiWeaponItemDetails.chat_link + "\n";
                    ItemIconLink = apiWeaponItemDetails.icon;
                    break;
                case "Back":
                    var apiBackItemDetails = JsonConvert.DeserializeObject<BackItem.RootObject>(ApiResponse);
                    ItemDetailsText = "Name: " + apiBackItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiBackItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiBackItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiBackItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiBackItemDetails.vendor_value + "\n";
                    ItemIconLink = apiBackItemDetails.icon;
                    break;
                case "Bag":
                    var apiBagItemDetails = JsonConvert.DeserializeObject<Bag.RootObject>(ApiResponse);
                    ItemDetailsText = "Name: " + apiBagItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiBagItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiBagItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiBagItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiBagItemDetails.vendor_value + "\n";
                    ItemIconLink = apiBagItemDetails.icon;
                    break;
                case "Consumable":
                    var apiConsumableItemDetails = JsonConvert.DeserializeObject<Consumable.RootObject>(ApiResponse);
                    ItemDetailsText = "Name: " + apiConsumableItemDetails.name + "\n";
                    ItemDetailsText += "Item type: " + apiConsumableItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiConsumableItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiConsumableItemDetails.vendor_value + "\n";
                    ItemIconLink = apiConsumableItemDetails.icon;
                    break;
                case "Container":
                    var apiContainerItemTypeDetails = JsonConvert.DeserializeObject<ContainerItemType.RootObject>(ApiResponse);
                    ItemDetailsText = "Name: " + apiContainerItemTypeDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiContainerItemTypeDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiContainerItemTypeDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiContainerItemTypeDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiContainerItemTypeDetails.vendor_value + "\n";
                    ItemIconLink = apiContainerItemTypeDetails.icon;
                    break;
                case "CraftingMaterial":
                    var apiCraftingMaterialItemDetails = JsonConvert.DeserializeObject<CraftingMaterial.RootObject>(ApiResponse);
                    ItemDetailsText = "Name: " + apiCraftingMaterialItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiCraftingMaterialItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiCraftingMaterialItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiCraftingMaterialItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiCraftingMaterialItemDetails.vendor_value + "\n";
                    ItemIconLink = apiCraftingMaterialItemDetails.icon;
                    break;
                case "Gathering":
                    var apiGatheringItemDetails = JsonConvert.DeserializeObject<Gathering.RootObject>(ApiResponse);
                    ItemDetailsText = "Name: " + apiGatheringItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiGatheringItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiGatheringItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiGatheringItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiGatheringItemDetails.vendor_value + "\n";
                    ItemIconLink = apiGatheringItemDetails.icon;
                    break;
                case "Gizmo":
                    var apiGizmoItemDetails = JsonConvert.DeserializeObject<Gizmo.RootObject>(ApiResponse);
                    ItemDetailsText = "Name: " + apiGizmoItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiGizmoItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiGizmoItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiGizmoItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiGizmoItemDetails.vendor_value + "\n";
                    ItemIconLink = apiGizmoItemDetails.icon;
                    break;
                case "MiniPet":
                    var apiMiniPetItemDetails = JsonConvert.DeserializeObject<MiniPet.RootObject>(ApiResponse);
                    ItemDetailsText = "Name: " + apiMiniPetItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiMiniPetItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiMiniPetItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiMiniPetItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiMiniPetItemDetails.vendor_value + "\n";
                    ItemIconLink = apiMiniPetItemDetails.icon;
                    break;
                case "Tool":
                    var apiToolItemDetails = JsonConvert.DeserializeObject<Tool.RootObject>(ApiResponse);
                    ItemDetailsText = "Name: " + apiToolItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiToolItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiToolItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiToolItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiToolItemDetails.vendor_value + "\n";
                    ItemIconLink = apiToolItemDetails.icon;
                    break;
                case "Trinket":
                    var apiTrinketItemDetails = JsonConvert.DeserializeObject<Trinket.RootObject>(ApiResponse);
                    ItemDetailsText = "Name: " + apiTrinketItemDetails.name + "\n";
                    ItemDetailsText += "Item type: " + apiTrinketItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiTrinketItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiTrinketItemDetails.vendor_value + "\n";
                    ItemIconLink = apiTrinketItemDetails.icon;
                    break;
                case "Trophy":
                    var apiTrophyItemDetails = JsonConvert.DeserializeObject<Trophy.RootObject>(ApiResponse);
                    ItemDetailsText = "Name: " + apiTrophyItemDetails.name + "\n";
                    ItemDetailsText += "Description: " + apiTrophyItemDetails.description + "\n";
                    ItemDetailsText += "Item type: " + apiTrophyItemDetails.type + "\n";
                    ItemDetailsText += "Rarity: " + apiTrophyItemDetails.rarity + "\n";
                    ItemDetailsText += "Vendor value: " + apiTrophyItemDetails.vendor_value + "\n";
                    ItemIconLink = apiTrophyItemDetails.icon;
                    break;
                case "UpgradeComponent":
                    var apiUpgradeComponentItemDetails = JsonConvert.DeserializeObject<UpgradeComponent.RootObject>(ApiResponse);
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

        // gets API item details response
        async Task<bool> GetApiItemDetailsResponse(string apiItemLink)
        {
            try
            {
                ApiResponse = await InternetConnection.client.GetStringAsync(apiItemLink);
            }
            catch (HttpRequestException)
            {
                TradingPostStatusText = "No such id";
                IsTradingPostStatusTextVisible = true;
                return false;
            }
            catch (Exception)
            {
                TradingPostStatusText = "Unknown exception!";
                IsTradingPostStatusTextVisible = true;
                return false;
            }
            return true;
        }

        // gets item type from API response
        bool GetItemType()
        {
            try
            {
                ApiResponseItemType = JsonConvert.DeserializeObject<ItemType>(ApiResponse);
            }
            catch (JsonSerializationException)
            {
                TradingPostStatusText = "JsonSerialization exception!";
                IsTradingPostStatusTextVisible = true;
                return false;
            }
            return true;
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
