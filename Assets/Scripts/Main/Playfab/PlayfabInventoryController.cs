using System.Collections;
using System.Collections.Generic;
using Core.Character;
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
        
        //initialize items
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), ItemsInitialized, error =>
        {
            error.LogToUnity("Get user inventory request failed");
        });
        
        //initialize slots
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), SlotsInitialized, error =>
        {
            error.LogToUnity("Get user slots request failed");
        });

        void ItemsInitialized(GetUserInventoryResult result)
        {
            (items, currency) = this.ItemsInitialized(result);

            itemsInitialized = true;

            //both items and slots initialized
            if (slotsInitialized)
                InventoryValuesInitialized(items, slots, currency);
        }

        void SlotsInitialized(GetUserDataResult result)
        {
            slots = this.SlotsInitialized(result);

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
