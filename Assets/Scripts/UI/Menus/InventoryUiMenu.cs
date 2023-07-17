using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using Ui.Main;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class InventoryUiMenu : UiMenu
{
    [SerializeField] private AssetReference _itemUiPrefabReference;
    
    [SerializeField] private Transform _container;
    
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

        _inventoryController.Bag.OnNewItemAdded += AttachItemUi;
    }

    void AttachItemUi(IItemData itemData)
    {
        Utils.LoadObjComponent<ItemUiAdapter>(_itemUiPrefabReference.AssetGUID, itemUiAdapter =>
        {
            itemUiAdapter = Instantiate(itemUiAdapter, _container);
            
            itemUiAdapter.AttachInventoryItem(itemData);
        });
    }
}
