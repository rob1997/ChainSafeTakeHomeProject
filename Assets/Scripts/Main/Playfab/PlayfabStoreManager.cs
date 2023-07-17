using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Game;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using Unity.Netcode;
using UnityEngine;

public class PlayfabStoreManager : StoreManager
{
    public const string CatalogVersion = "1.0";

    private List<PlayfabItemData> _allItems = new List<PlayfabItemData>();

    public override List<IItemData> AllItems => _allItems.ConvertAll(i => (IItemData) i);

    protected override void InitializeStore()
    {
        var catalogRequest = new GetCatalogItemsRequest
        {
            CatalogVersion = CatalogVersion
        };
        
        PlayFabClientAPI.GetCatalogItems(catalogRequest, StoreInitialized, error =>
        {
            error.LogToUnity("failed to get catalog items");
        });
        
        void StoreInitialized(GetCatalogItemsResult result)
        {
            _allItems = result.Catalog.ConvertAll(PlayfabItemData.Create);
            
            Debug.Log("Catalog items initialized");
            
            InvokeStoreInitialized();
        }
    }

    public override void BuyItem(string itemId)
    {
        //if item doesn't exist return
        if (!GetItem(itemId, out IItemData itemData))
            return;
        
        var purchaseRequest = new PurchaseItemRequest
        {
            CatalogVersion = CatalogVersion,
            
            ItemId = itemId,
            
            VirtualCurrency = PlayfabUtils.CoinCurrencyKey,
            
            Price = itemData.Price
        };

        PlayFabClientAPI.PurchaseItem(purchaseRequest, ItemPurchased, error =>
        {
            error.LogToUnity($"failed purchasing item {itemId}");
        });
    }

    private void ItemPurchased(PurchaseItemResult result)
    {
        foreach (ItemInstance item in result.Items)
        {
            if (GetItem(item.ItemId, out var itemData))
            {
                ItemPurchased(itemData);
            }
        }
    }

    protected override void ItemPurchased(IItemData itemData)
    {
        InventoryController.Bag.ItemPurchased(itemData, itemData.Price);
            
        Debug.Log($"{itemData.DisplayName} item purchased");
    }
}
