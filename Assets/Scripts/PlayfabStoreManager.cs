using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Game;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayfabStoreManager : Manager<PlayfabStoreManager>
{
    public const string CatalogVersion = "1.0";

    public static string CoinCurrencyKey = "CN";
    
    public List<PlayfabStoreItemData> AllItems { get; private set; } = new List<PlayfabStoreItemData>();

    #region StoreInitialized

    public delegate void StoreInitialized();

    public event StoreInitialized OnStoreInitialized;

    private void InvokeStoreInitialized()
    {
        IsStoreInitialized = true;
        
        OnStoreInitialized?.Invoke();
    }

    #endregion

    public bool IsStoreInitialized { get; private set; }
    
    public override void Initialize()
    {
        if (PlayfabUserManager.Instance.IsAuthenticated)
            InitializeCatalog();

        else
            PlayfabUserManager.Instance.OnAuthenticated += InitializeCatalog;
    }

    private void InitializeCatalog()
    {
        var catalogRequest = new GetCatalogItemsRequest
        {
            CatalogVersion = CatalogVersion
        };
        
        PlayFabClientAPI.GetCatalogItems(catalogRequest, CatalogInitialized, error =>
        {
            PlayfabUserManager.LogFailedRequest("failed to get catalog items", error);
        });
        
        void CatalogInitialized(GetCatalogItemsResult result)
        {
            foreach (var item in result.Catalog)
            {
                PlayfabStoreItemData itemData = JsonConvert.DeserializeObject<PlayfabStoreItemData>(item.CustomData);
                
                itemData.SetPlayfabItem(item);
                
                AllItems.Add(itemData);
            }
            
            Debug.Log("Catalog items initialized");
            
            InvokeStoreInitialized();
        }
    }

    public bool GetItem(string itemId, out PlayfabStoreItemData itemData)
    {
        itemData = default;

        if (AllItems.Any(i => i.Id == itemId))
        {
            itemData = AllItems.SingleOrDefault(i => i.Id == itemId);
            
            return true;
        }

        return false;
    }
    
    public void BuyItem(string itemId)
    {
        var item = AllItems.FirstOrDefault(i => i.Id == itemId);

        var purchaseRequest = new PurchaseItemRequest
        {
            CatalogVersion = CatalogVersion,
            
            ItemId = itemId,
            
            VirtualCurrency = CoinCurrencyKey,
            
            Price = item.Price
        };

        PlayFabClientAPI.PurchaseItem(purchaseRequest, ItemPurchased, error =>
        {
            PlayfabUserManager.LogFailedRequest($"failed purchasing item {itemId}", error);
        });
    }

    private void ItemPurchased(PurchaseItemResult result)
    {
        Player.Instance.GetController(out InventoryController inventoryController);

        foreach (ItemInstance item in result.Items)
        {
            GetItem(item.ItemId, out var shopItem);
            
            PlayfabItemData itemData = JsonConvert.DeserializeObject<PlayfabItemData>(shopItem.PlayfabItem.CustomData);
            
            itemData.SetPlayfabItem(item);
            
            inventoryController.Bag.PurchaseItem(itemData, shopItem.Price);
            
            Debug.Log($"{item.DisplayName} item purchased");
        }
    }
}
