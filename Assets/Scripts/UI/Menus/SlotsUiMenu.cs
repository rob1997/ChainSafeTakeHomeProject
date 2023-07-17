using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using Ui.Main;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class SlotsUiMenu : UiMenu
{
    [SerializeField] private bool _isNetworked;
    
    [SerializeField] private AssetReference _slotsUiAdapterPrefabReference;
    
    [SerializeField] private Transform _container;
    
    private Dictionary<ulong, SlotsUiAdapter> _slotsUiAdapters = new Dictionary<ulong, SlotsUiAdapter>();
    
    public override void Initialize(UiRegion rootUiElement)
    {
        base.Initialize(rootUiElement);

        if (_isNetworked)
        {
            foreach (var player in Player.SpawnedPlayers)
            {
                InitializePlayerUiSlots(player);
            }

            Player.OnPlayerSpawned += InitializePlayerUiSlots;

            Player.OnPlayerDeSpawned += DeSpawnPlayerSlotsUi;
        }

        //only add local player
        else
            InitializePlayerUiSlots(Player.Instance);
    }

    private void DeSpawnPlayerSlotsUi(ulong ownerClientId)
    {
        if (!_slotsUiAdapters.ContainsKey(ownerClientId))
            return;
        
        Destroy(_slotsUiAdapters[ownerClientId].gameObject);
            
        _slotsUiAdapters.Remove(ownerClientId);
    }

    private void InitializePlayerUiSlots(Player player)
    {
        if (player.IsReady)
            AddSlotsUi();
            
        else
            player.OnReady += AddSlotsUi;
        
        void AddSlotsUi()
        {
            Utils.LoadObjComponent<SlotsUiAdapter>(_slotsUiAdapterPrefabReference.AssetGUID, slotsUiAdapter =>
            {
                slotsUiAdapter = Instantiate(slotsUiAdapter, _container);
            
                slotsUiAdapter.Attach(player);
            
                _slotsUiAdapters.Add(player.OwnerClientId, slotsUiAdapter);
            });
        }
    }
}
