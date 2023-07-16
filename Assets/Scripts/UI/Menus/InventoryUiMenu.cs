using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using Ui.Main;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

[Serializable]
public struct SlotUi
{
    [field: SerializeField] public Image Image { get; private set; }
    
    [field: SerializeField] public Button UnEquipButton { get; private set; }
}

public class InventoryUiMenu : UiMenu
{
    [SerializeField] private AssetReference _itemUiPrefabReference;
    
    [SerializeField] private Transform _container;

    [HideInInspector] public GenericDictionary<ItemCategory, SlotUi> SlotsUiLookup = GenericDictionary<ItemCategory, SlotUi>
        .ToGenericDictionary(Utils.GetEnumValues<ItemCategory>().ToDictionary(c => c, c => default(SlotUi)));
    
    private InventoryController _inventoryController;
    
    public override void Initialize(UiRegion rootUiElement)
    {
        base.Initialize(rootUiElement);

        Player.Instance.GetController(out _inventoryController);

        if (_inventoryController.IsInventoryInitialized)
            InitializeInventoryUi();

        else
            _inventoryController.OnInventoryInitialized += InitializeInventoryUi;
    }

    private void InitializeInventoryUi()
    {
        foreach (var itemData in _inventoryController.Bag.AllItems)
        {
            AttachItemUi(itemData);
        }

        //initialize slots
        foreach (var pair in _inventoryController.Bag.Slots)
        {
            if (!string.IsNullOrEmpty(pair.Value))
            {
                EquipUiItem(pair.Value);
            }
        }
        
        _inventoryController.Bag.OnNewItemAdded += AttachItemUi;
        
        _inventoryController.Bag.OnItemEquipped += EquipUiItem;
        
        _inventoryController.Bag.OnSlotUnEquipped += UnEquipUiSlot;
        
        foreach (var pair in SlotsUiLookup)
        {
            pair.Value.UnEquipButton.onClick.AddListener(delegate { UnEquipSlot(pair.Key); });
        }
    }

    void AttachItemUi(IItemData itemData)
    {
        Utils.LoadObjComponent<ItemUiAdapter>(_itemUiPrefabReference.AssetGUID, itemUiAdapter =>
        {
            itemUiAdapter = Instantiate(itemUiAdapter, _container);
            
            itemUiAdapter.AttachInventoryItem(itemData);
        });
    }
    
    private void UnEquipUiSlot(ItemCategory category)
    {
        SlotsUiLookup[category].Image.sprite = null;
    }

    private void EquipUiItem(string itemId)
    {
        var itemData = _inventoryController.Bag.AllItems.FirstOrDefault(i => i.Id == itemId);
        
        EquipUiItem(itemData);
    }
    
    private void EquipUiItem(IItemData itemData)
    {
        Utils.LoadAsset<Texture2D>(itemData.SpriteAssetPath, result =>
        {
            SlotsUiLookup[itemData.Category].Image.sprite = Sprite.Create(result, new Rect(0f, 0f, result.width, result.height), Vector2.zero);
        });
    }
    
    private void UnEquipSlot(ItemCategory category)
    {
        _inventoryController.UnEquipSlot(category);
    }
}
