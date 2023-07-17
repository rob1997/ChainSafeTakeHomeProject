using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Character;
using Core.Utils;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayfabInventoryController : InventoryController
{
    public override Bag Bag { get; protected set; } = new PlayfabBag();
    
    protected override void InitializeInventory()
    {
        IItemData[] items = new IItemData[0];
        
        int currency = 0;

        Dictionary<ItemCategory, string> slots = new Dictionary<ItemCategory, string>();

        bool itemsInitialized = false;
        
        bool slotsInitialized = false;

        var inventoryRequest = new ExecuteCloudScriptRequest
        {
            FunctionName = PlayfabUtils.GetUserInventoryCloudFunctionName,

            FunctionParameter = Player.NetworkId

        };
        
        //get user inventory
        PlayFabClientAPI.ExecuteCloudScript(inventoryRequest, ItemsInitialized, error =>
        {
            error.LogToUnity($"{PlayfabUtils.GetUserInventoryCloudFunctionName} cloud function failed");
        });
        
        var characterDataRequest = new ExecuteCloudScriptRequest
        {
            FunctionName = PlayfabUtils.GetUserDataCloudFunctionName,
            
            FunctionParameter = Player.NetworkId
        };
        
        //get user data
        PlayFabClientAPI.ExecuteCloudScript(characterDataRequest, SlotsInitialized, error =>
        {
            error.LogToUnity($"{PlayfabUtils.GetUserDataCloudFunctionName} cloud function failed");
        });

        void ItemsInitialized(ExecuteCloudScriptResult result)
        {
            var inventoryResult = JsonConvert.DeserializeObject<GetUserInventoryResult>(result.FunctionResult.ToString());
            
            (items, currency) = this.ItemsInitialized(inventoryResult);

            itemsInitialized = true;

            //both items and slots initialized
            if (slotsInitialized)
                InventoryValuesInitialized(items, slots, currency);
        }

        void SlotsInitialized(ExecuteCloudScriptResult result)
        {
            var userDataResult = JsonConvert.DeserializeObject<GetUserDataResult>(result.FunctionResult.ToString());

            //check if slots were initialized
            slots = userDataResult.Data.ContainsKey(nameof(Bag.Slots)) ? 
                this.SlotsInitialized(userDataResult) : Utils.GetEnumValues<ItemCategory>().ToDictionary(c => c, c => string.Empty);

            slotsInitialized = true;

            //both items and slots initialized
            if (itemsInitialized)
                InventoryValuesInitialized(items, slots, currency);
        }
    }

    private Dictionary<ItemCategory, string> SlotsInitialized(GetUserDataResult result)
    {
        return JsonConvert.DeserializeObject<Dictionary<ItemCategory, string>>(result.Data[nameof(Bag.Slots)].Value);
    }

    private (IItemData[], int) ItemsInitialized(GetUserInventoryResult result)
    {
        int currency = result.VirtualCurrency[PlayfabUtils.CoinCurrencyKey];
        
        var inventoryItems = result.Inventory;
        
        IItemData[] items = new IItemData[inventoryItems.Count];

        for (int i = 0; i < items.Length; i++)
        {
            var itemInstance = inventoryItems[i];
            
            if (StoreManager.Instance.GetItem(itemInstance.ItemId, out var itemData))
            {
                items[i] = itemData;
            }
        }

        return (items, currency);
    }
    
    public override void EquipItem(string itemId)
    {
        Bag.EquipItem(itemId);
    }

    public override void UnEquipSlot(ItemCategory category)
    {
        Bag.UnEquipSlot(category);
    }
}
