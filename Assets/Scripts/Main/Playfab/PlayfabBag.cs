using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayfabBag : Bag
{
    private readonly List<PlayfabItemData> _allItems = new List<PlayfabItemData>();

    public override List<IItemData> AllItems => _allItems.ConvertAll(i => (IItemData) i);

    //update player data
    private UpdateUserDataRequest UpdateSlotsRequest => new UpdateUserDataRequest
    {
        Data = new Dictionary<string, string>
        {
            { nameof(Slots), JsonConvert.SerializeObject(Slots) }
        }
    };
    
    public override void AddItem(IItemData itemData, bool silent = false)
    {
        _allItems.Add((PlayfabItemData) itemData);

        if (!silent) InvokeNewItemAdded(itemData);
    }

    public override void EquipItem(string itemId)
    {
        IItemData itemData = AllItems.FirstOrDefault(i => i.Id == itemId);
        
        //null item or already equipped
        if (itemData == null || Slots[itemData.Category] == itemId)
            return;

        //cache this value to revert in case of request fail
        string equippedItemId = Slots[itemData.Category];
        
        Slots[itemData.Category] = itemData.Id;
        
        PlayFabClientAPI.UpdateUserData(UpdateSlotsRequest, Equipped, error =>
        {
            //revert value
            Slots[itemData.Category] = equippedItemId;
            
            error.LogToUnity($"equipping item {itemData.DisplayName} failed");
        });

        void Equipped(UpdateUserDataResult result)
        {
            this.Equipped(itemData);
        }
    }

    protected override void Equipped(IItemData itemData)
    {
        InvokeItemEquipped(itemData);
            
        Debug.Log($"equipped item {itemData.DisplayName}");
    }
    
    public override void UnEquipSlot(ItemCategory category)
    {
        //check if already unequipped
        if (string.IsNullOrEmpty(Slots[category]))
            return;
        
        //cache this value to revert in case of request fail
        string equippedItemId = Slots[category];
        
        Slots[category] = string.Empty;
        
        PlayFabClientAPI.UpdateUserData(UpdateSlotsRequest, UnEquipped, error =>
        {
            //revert value
            Slots[category] = equippedItemId;
            
            error.LogToUnity($"un equipping slot {category} failed");
        });
        
        void UnEquipped(UpdateUserDataResult result)
        {
            this.UnEquipped(category);
        }
    }

    protected override void UnEquipped(ItemCategory category)
    {
        InvokeSlotUnEquipped(category);
            
        Debug.Log($"un equipped slot {category}");
    }
}
