using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Character;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class InventoryController : Controller
{
    #region InventoryInitialized

    public delegate void InventoryInitialized();

    public event InventoryInitialized OnInventoryInitialized;

    private void InvokeInventoryInitialized()
    {
        IsInventoryInitialized = true;
        
        Debug.Log("inventory initialized");
        
        OnInventoryInitialized?.Invoke();
    }

    #endregion

    public bool IsInventoryInitialized { get; private set; }

    private bool _itemsInitialized;
    
    private bool _slotsInitialized;
    
    public Bag Bag { get; private set; } = new Bag();
    
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
            PlayfabUserManager.LogFailedRequest("Get user inventory request failed", error);
        });
        
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), SlotsInitialized, error =>
        {
            PlayfabUserManager.LogFailedRequest("Get user slots request failed", error);
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

        Bag.SetCurrency(result.VirtualCurrency[PlayfabStoreManager.CoinCurrencyKey]);
        
        foreach (var item in inventoryItems)
        {
            PlayfabStoreManager.Instance.GetItem(item.ItemId, out var shopItem);
            
            PlayfabItemData itemData = JsonConvert.DeserializeObject<PlayfabItemData>(shopItem.PlayfabItem.CustomData);
            
            itemData.SetPlayfabItem(item);
            
            Bag.AddItem(itemData, true);
        }
        
        _itemsInitialized = true;
        
        Debug.Log("inventory items initialized");
        
        if (_slotsInitialized) InvokeInventoryInitialized();
    }

    public void EquipItem(string itemId)
    {
        Bag.EquipItem(itemId);
    }

    public void UnEquipSlot(ItemCategory category)
    {
        Bag.UnEquipSlot(category);
    }
}
