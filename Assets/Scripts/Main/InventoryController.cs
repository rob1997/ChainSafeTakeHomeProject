using System.Collections.Generic;
using Core.Character;
using Unity.Netcode;
using UnityEngine;

public abstract class InventoryController : Controller
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
    
    public abstract Bag Bag { get; protected set; }

    protected Player Player { get; private set; }
    
    public override void Initialize(Character character)
    {
        base.Initialize(character);

        Player = (Player) character;
        
        if (!StoreManager.Instance.IsStoreInitialized)
        {
            StoreManager.Instance.OnStoreInitialized += InitializeInventory;
        }

        else
            InitializeInventory();
    }

    protected abstract void InitializeInventory();

    protected void InventoryValuesInitialized(IItemData[] items, Dictionary<ItemCategory, string> slots, int currency)
    {
        Bag.Initialize(items, slots, currency);
        
        InvokeInventoryInitialized();
    }
    
    public abstract void EquipItem(string itemId);

    public abstract void UnEquipSlot(ItemCategory category);
}
