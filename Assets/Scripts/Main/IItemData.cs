using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public enum ItemCategory
{
    Hat = 0,
    
    Shirt = 1,
    
    Pants = 2,
    
    Shoe = 3,
}

public interface IItemData : IDataModel
{
    string Id { get; }
    
    string DisplayName { get; }
    
    ItemCategory Category { get; }
    
    int Price { get; }
    
    string SpriteAssetPath { get; }
}
