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
        IItemData item = AllItems.FirstOrDefault(i => i.Id == itemId);
        
        //already equipped
        if (item == null || Slots[item.Category] == itemId)
            return;
        
        Slots[item.Category] = item.Id;
        
        PlayFabClientAPI.UpdateUserData(UpdateSlotsRequest, Equipped, error =>
        {
            error.LogToUnity($"equipping item {item.DisplayName} failed");
        });

        void Equipped(UpdateUserDataResult result)
        {
            InvokeItemEquipped(item);
            
            Debug.Log($"equipped item {item.DisplayName}");
        }
    }

    public override void UnEquipSlot(ItemCategory category)
    {
        //check if already unequipped
        if (string.IsNullOrEmpty(Slots[category]))
            return;
        
        Slots[category] = string.Empty;
        
        PlayFabClientAPI.UpdateUserData(UpdateSlotsRequest, UnEquipped, error =>
        {
            error.LogToUnity($"un equipping slot {category} failed");
        });
        
        void UnEquipped(UpdateUserDataResult result)
        {
            InvokeSlotUnEquipped(category);
            
            Debug.Log($"un equipped slot {category}");
        }
    }
}
