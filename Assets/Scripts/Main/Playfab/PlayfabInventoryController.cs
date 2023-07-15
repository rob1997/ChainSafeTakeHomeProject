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
    
    private bool _itemsInitialized;
    
    private bool _slotsInitialized;
    
    public override void Initialize(Character character)
    {
        base.Initialize(character);
        
        if (!PlayfabStoreManager.Instance.IsStoreInitialized)
        {
            PlayfabStoreManager.Instance.OnStoreInitialized += InitializeItems;
        }

        else
            InitializeItems();
    }

    private void InitializeItems()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), ItemsInitialized, error =>
        {
            error.LogToUnity("Get user inventory request failed");
        });
        
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), SlotsInitialized, error =>
        {
            error.LogToUnity("Get user slots request failed");
        });
    }

    private void SlotsInitialized(GetUserDataResult result)
    {
        var slots = JsonConvert.DeserializeObject<Dictionary<ItemCategory, string>>(result.Data[nameof(Bag.Slots)].Value);
        
        Bag.InitializeSlots(slots);

        _slotsInitialized = true;
        
        Debug.Log("slots initialized");
        
        if (_itemsInitialized) InvokeInventoryInitialized();
    }

    private void ItemsInitialized(GetUserInventoryResult result)
    {
        var inventoryItems = result.Inventory;

        Bag.InitializeCurrency(result.VirtualCurrency[PlayfabUtils.CoinCurrencyKey]);
        
        foreach (var item in inventoryItems)
        {
            if (PlayfabStoreManager.Instance.GetItem(item.ItemId, out var itemData))
            {
                Bag.AddItem(itemData, true);
            }
        }
        
        _itemsInitialized = true;
        
        Debug.Log("inventory items initialized");
        
        if (_slotsInitialized) InvokeInventoryInitialized();
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
