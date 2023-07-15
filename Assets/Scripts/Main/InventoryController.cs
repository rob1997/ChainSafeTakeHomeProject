using Core.Character;
using UnityEngine;

public abstract class InventoryController : Controller
{
    #region InventoryInitialized

    public delegate void InventoryInitialized();

    public event InventoryInitialized OnInventoryInitialized;

    protected void InvokeInventoryInitialized()
    {
        IsInventoryInitialized = true;
        
        Debug.Log("inventory initialized");
        
        OnInventoryInitialized?.Invoke();
    }

    #endregion

    public bool IsInventoryInitialized { get; private set; }
    
    public abstract Bag Bag { get; protected set; }

    public abstract void EquipItem(string itemId);

    public abstract void UnEquipSlot(ItemCategory category);
}
