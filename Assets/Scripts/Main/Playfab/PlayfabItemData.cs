using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab.ClientModels;
using UnityEngine;

[Serializable]
public struct PlayfabItemData : IItemData
{
    [JsonIgnore] public string Id { get; private set; }

    [JsonIgnore] public string DisplayName { get; private set; }
    
    [JsonIgnore] public int Price { get; private set; }

    [JsonProperty("category")]
    public ItemCategory Category { get; private set; }

    [JsonProperty("spriteAssetPath")]
    public string SpriteAssetPath { get; private set; }

    public static PlayfabItemData Create(CatalogItem catalogItem)
    {
        var itemData = JsonConvert.DeserializeObject<PlayfabItemData>(catalogItem.CustomData);

        itemData.Id = catalogItem.ItemId;
        
        itemData.DisplayName = catalogItem.DisplayName;
        
        itemData.Price = (int) catalogItem.VirtualCurrencyPrices[PlayfabUtils.CoinCurrencyKey];

        return itemData;
    }
    
    public static PlayfabItemData Create(ItemInstance itemInstance)
    {
        StoreManager.Instance.GetItem(itemInstance.ItemId, out IItemData itemData);

        return (PlayfabItemData) itemData;
    }
}
