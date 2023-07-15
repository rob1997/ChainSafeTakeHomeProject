using System.Collections;
using System.Collections.Generic;
using Ui.Main;
using UnityEngine;

public class StoreUiMenu : UiMenu
{
    [SerializeField] private ItemUiAdapter _itemUiPrefab;
    
    [SerializeField] private Transform _container;
    
    public override void Initialize(UiRegion rootUiElement)
    {
        base.Initialize(rootUiElement);

        if (PlayfabStoreManager.Instance.IsStoreInitialized)
            InitializeStoreUi();

        else
            PlayfabStoreManager.Instance.OnStoreInitialized += InitializeStoreUi;
    }

    private void InitializeStoreUi()
    {
        foreach (var itemData in PlayfabStoreManager.Instance.AllItems)
        {
            ItemUiAdapter itemUiAdapter = Instantiate(_itemUiPrefab, _container);
            
            itemUiAdapter.Attach(itemData, true);
        }
    }
}
