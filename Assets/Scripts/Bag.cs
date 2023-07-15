using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class Bag
{
    #region NewItemAdded

    public delegate void NewItemAdded(PlayfabItemData itemData);

    public event NewItemAdded OnNewItemAdded;

    private void InvokeNewItemAdded(PlayfabItemData itemData)
    {
        OnNewItemAdded?.Invoke(itemData);
    }

    #endregion
    
    #region CurrencyUpdated

    public delegate void CurrencyUpdated(int change);

    public event CurrencyUpdated OnCurrencyUpdated;

    private void InvokeCurrencyUpdated(int change)
    {
        OnCurrencyUpdated?.Invoke(change);
    }

    #endregion

    #region ItemEquipped

    public delegate void ItemEquipped(PlayfabItemData itemData);

    public event ItemEquipped OnItemEquipped;

    public void InvokeItemEquipped(PlayfabItemData itemData)
    {
        OnItemEquipped?.Invoke(itemData);
    }

    #endregion

    #region SlotUnEquipped

    public delegate void SlotUnEquipped(ItemCategory category);

    public event SlotUnEquipped OnSlotUnEquipped;

    private void InvokeSlotUnEquipped(ItemCategory category)
    {
        OnSlotUnEquipped?.Invoke(category);
    }

    #endregion
    
    public int Currency { get; private set; }
    
    //key is the slot and value is itemId
    public Dictionary<ItemCategory, string> Slots { get; private set; } = Utils.GetEnumValues<ItemCategory>().ToDictionary(c => c, c => string.Empty);
    
    private readonly List<PlayfabItemData> _allItems = new List<PlayfabItemData>();

    public List<PlayfabItemData> AllItems => _allItems;

    //update player data
    public UpdateUserDataRequest UpdateSlotsRequest => new UpdateUserDataRequest
    {
        Data = new Dictionary<string, string>
        {
            { nameof(Slots), JsonConvert.SerializeObject(Slots) }
        }
    };
    
    public void InitializeSlots(Dictionary<ItemCategory, string> slots)
    {
        Slots = slots;
    }
    
    public void AddItem(PlayfabItemData item, bool silent = false)
    {
        _allItems.Add(item);

        if (!silent) InvokeNewItemAdded(item);
    }
    
    public void PurchaseItem(PlayfabItemData item, int amount)
    {
        AddItem(item);

        Currency -= amount;
        
        InvokeCurrencyUpdated(- amount);
    }

    public void SetCurrency(int currency)
    {
        Currency = currency;
    }

    public void EquipItem(string itemId)
    {
        var item = AllItems.FirstOrDefault(i => i.Id == itemId);

        //already equipped
        if (Slots[item.Category] == itemId)
            return;
        
        Slots[item.Category] = item.Id;
        
        PlayFabClientAPI.UpdateUserData(UpdateSlotsRequest, Equipped, error =>
        {
            PlayfabUserManager.LogFailedRequest($"equipping item {item.DisplayName} failed", error);
        });

        void Equipped(UpdateUserDataResult result)
        {
            InvokeItemEquipped(item);
            
            Debug.Log($"equipped item {item.DisplayName}");
        }
    }

    public void UnEquipSlot(ItemCategory category)
    {
        //check if already unequipped
        if (string.IsNullOrEmpty(Slots[category]))
            return;
        
        Slots[category] = string.Empty;
        
        PlayFabClientAPI.UpdateUserData(UpdateSlotsRequest, UnEquipped, error =>
        {
            PlayfabUserManager.LogFailedRequest($"un equipping slot {category} failed", error);
        });
        
        void UnEquipped(UpdateUserDataResult result)
        {
            InvokeSlotUnEquipped(category);
            
            Debug.Log($"un equipped slot {category}");
        }
    }
}
