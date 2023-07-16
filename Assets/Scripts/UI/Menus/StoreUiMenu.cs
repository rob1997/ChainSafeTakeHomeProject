using System.Collections;
using System.Collections.Generic;
using Core.Utils;
using Ui.Main;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class StoreUiMenu : UiMenu
{
    [SerializeField] private AssetReference _itemUiPrefabReference;
    
    [SerializeField] private Transform _container;
    
    public override void Initialize(UiRegion rootUiElement)
    {
        base.Initialize(rootUiElement);

        if (StoreManager.Instance.IsStoreInitialized)
            InitializeStoreUi();

        else
            StoreManager.Instance.OnStoreInitialized += InitializeStoreUi;
    }

    private void InitializeStoreUi()
    {
        Utils.LoadObjComponent<ItemUiAdapter>(_itemUiPrefabReference.AssetGUID, itemUiAdapter =>
        {
            foreach (var itemData in StoreManager.Instance.AllItems)
            {
                itemUiAdapter = Instantiate(itemUiAdapter, _container);
            
                itemUiAdapter.AttachStoreItem(itemData);
            }
        });
    }
}
