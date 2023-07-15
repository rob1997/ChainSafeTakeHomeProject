using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab.ClientModels;
using UnityEngine;

public struct PlayfabStoreItemData : IItemData
{
    [JsonIgnore] public string Id => PlayfabItem.ItemId;

    [JsonIgnore] public int Price => (int) PlayfabItem.VirtualCurrencyPrices[PlayfabStoreManager.CoinCurrencyKey];
    
    [JsonIgnore] public string DisplayName => PlayfabItem.DisplayName;

    [JsonProperty("category")]
    public ItemCategory Category { get; private set; }
    
    [JsonProperty("spriteAssetUrl")]
    public string SpriteAssetPath { get; private set; }
    
    [JsonIgnore] public CatalogItem PlayfabItem { get; private set; }

    public void SetPlayfabItem(CatalogItem playfabItem)
    {
        PlayfabItem = playfabItem;
    }
}
