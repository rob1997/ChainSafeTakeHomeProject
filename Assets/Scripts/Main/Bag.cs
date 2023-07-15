using System.Collections.Generic;
using System.Linq;
using Core.Utils;

public abstract class Bag
{
    #region NewItemAdded

    public delegate void NewItemAdded(IItemData itemData);

    public event NewItemAdded OnNewItemAdded;

    protected void InvokeNewItemAdded(IItemData itemData)
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

    public delegate void ItemEquipped(IItemData itemData);

    public event ItemEquipped OnItemEquipped;

    protected void InvokeItemEquipped(IItemData itemData)
    {
        OnItemEquipped?.Invoke(itemData);
    }

    #endregion

    #region SlotUnEquipped

    public delegate void SlotUnEquipped(ItemCategory category);

    public event SlotUnEquipped OnSlotUnEquipped;

    protected void InvokeSlotUnEquipped(ItemCategory category)
    {
        OnSlotUnEquipped?.Invoke(category);
    }

    #endregion
    
    public int Currency { get; private set; }
    
    //key is the slot and value is itemId
    public Dictionary<ItemCategory, string> Slots { get; private set; } = Utils.GetEnumValues<ItemCategory>().ToDictionary(c => c, c => string.Empty);

    public abstract List<IItemData> AllItems { get; }
    
    public void InitializeCurrency(int currency)
    {
        Currency = currency;
    }
    
    public void InitializeSlots(Dictionary<ItemCategory, string> slots)
    {
        Slots = slots;
    }

    public abstract void AddItem(IItemData itemData, bool silent = false);
    
    public void ItemPurchased(IItemData itemData, int amount)
    {
        AddItem(itemData);

        Currency -= amount;
        
        InvokeCurrencyUpdated(- amount);
    }

    public abstract void EquipItem(string itemId);

    public abstract void UnEquipSlot(ItemCategory category);
}
