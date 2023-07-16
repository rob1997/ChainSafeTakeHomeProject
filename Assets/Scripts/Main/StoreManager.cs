using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Game;
using UnityEngine;

public abstract class StoreManager : Manager<StoreManager>
{
    #region StoreInitialized

    public delegate void StoreInitialized();
    
    public event StoreInitialized OnStoreInitialized;
    
    protected void InvokeStoreInitialized()
    {
        IsStoreInitialized = true;
        
        OnStoreInitialized?.Invoke();
    }

    #endregion
    
    public bool IsStoreInitialized { get; private set; }

    public abstract List<IItemData> AllItems { get; }
    
    protected InventoryController InventoryController
    {
        get
        {
            if (_inventoryController == null)
            {
                Player.Instance.GetController(out _inventoryController);
            }

            return _inventoryController;
        }
    }

    private InventoryController _inventoryController;
    
    public override void Initialize()
    {
        if (UserManager.Instance.IsAuthenticated)
            InitializeStore();

        else
            UserManager.Instance.OnAuthenticated += InitializeStore;
    }
    
    protected abstract void InitializeStore();
    
    public bool GetItem(string itemId, out IItemData itemData)
    {
        itemData = default;

        if (AllItems.Any(i => i.Id == itemId))
        {
            itemData = AllItems.SingleOrDefault(i => i.Id == itemId);
            
            return true;
        }
        
        return false;
    }
    
    public abstract void BuyItem(string itemId);

    protected abstract void ItemPurchased(IItemData itemData);
}
