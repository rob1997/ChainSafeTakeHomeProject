using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab.ClientModels;
using UnityEngine;

[Serializable]
public struct PlayfabItemData : IItemData
{
    [JsonIgnore] public string Id => PlayfabItem.ItemId;

    [JsonIgnore] public string DisplayName => PlayfabItem.DisplayName;

    [JsonProperty("category")]
    public ItemCategory Category { get; private set; }

    [JsonProperty("spriteAssetPath")]
    public string SpriteAssetPath { get; private set; }

    [JsonIgnore] public ItemInstance PlayfabItem { get; private set; }

    public void SetPlayfabItem(ItemInstance playfabItem)
    {
        PlayfabItem = playfabItem;
    }
}
