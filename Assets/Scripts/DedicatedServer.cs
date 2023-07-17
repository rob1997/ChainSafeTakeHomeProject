using System;
using System.Collections;
using System.Collections.Generic;
using Core.Utils;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Multiplay;
using UnityEngine;

public class DedicatedServer : Singleton<DedicatedServer>
{
    private float _autoAllocateTimer = 9999999f;
    private bool _alreadyAutoAllocated;

    private static IServerQueryHandler
        serverQueryHandler; // static so it doesn't get destroyed when this object is destroyed

    protected override void Awake()
    {
        base.Awake();

        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();

            await UnityServices.InitializeAsync(initializationOptions);

            Debug.Log("DEDICATED_SERVER LOBBY");

            MultiplayEventCallbacks multiplayEventCallbacks = new MultiplayEventCallbacks();
            multiplayEventCallbacks.Allocate += MultiplayEventCallbacks_Allocate;
            multiplayEventCallbacks.Deallocate += MultiplayEventCallbacks_Deallocate;
            multiplayEventCallbacks.Error += MultiplayEventCallbacks_Error;
            multiplayEventCallbacks.SubscriptionStateChanged += MultiplayEventCallbacks_SubscriptionStateChanged;
            IServerEvents serverEvents =
                await MultiplayService.Instance.SubscribeToServerEventsAsync(multiplayEventCallbacks);

            serverQueryHandler =
                await MultiplayService.Instance.StartServerQueryHandlerAsync(4, "ChainSafeTakeHomeProjectServer", "ChainSafeTakeHomeProjectGame", "1.0",
                    "Default");

            var serverConfig = MultiplayService.Instance.ServerConfig;
            if (serverConfig.AllocationId != "")
            {
                // Already Allocated
                MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId,
                    serverConfig.AllocationId));
            }
        }
        else
        {
            // Already Initialized
            Debug.Log("DEDICATED_SERVER LOBBY - ALREADY INIT");

            var serverConfig = MultiplayService.Instance.ServerConfig;
            if (serverConfig.AllocationId != "")
            {
                // Already Allocated
                MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId,
                    serverConfig.AllocationId));
            }
        }
    }

    private void MultiplayEventCallbacks_SubscriptionStateChanged(MultiplayServerSubscriptionState obj)
    {
        Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_SubscriptionStateChanged");
        Debug.Log(obj);
    }

    private void MultiplayEventCallbacks_Error(MultiplayError obj)
    {
        Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Error");
        Debug.Log(obj.Reason);
    }

    private void MultiplayEventCallbacks_Deallocate(MultiplayDeallocation obj)
    {
        Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Deallocate");
    }

    private void MultiplayEventCallbacks_Allocate(MultiplayAllocation obj)
    {
        Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Allocate");

        if (_alreadyAutoAllocated)
        {
            Debug.Log("Already auto allocated!");
            return;
        }

        _alreadyAutoAllocated = true;

        var serverConfig = MultiplayService.Instance.ServerConfig;
        Debug.Log($"Server ID[{serverConfig.ServerId}]");
        Debug.Log($"AllocationID[{serverConfig.AllocationId}]");
        Debug.Log($"Port[{serverConfig.Port}]");
        Debug.Log($"QueryPort[{serverConfig.QueryPort}]");
        Debug.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");

        string ipv4Address = "0.0.0.0";
        ushort port = serverConfig.Port;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port, "0.0.0.0");

        NetworkManager.Singleton.StartServer();

        MultiplayService.Instance.ReadyServerForPlayersAsync();
    }

    private void Update()
    {
        _autoAllocateTimer -= Time.deltaTime;
        if (_autoAllocateTimer <= 0f)
        {
            _autoAllocateTimer = 999f;
            MultiplayEventCallbacks_Allocate(null);
        }

        if (serverQueryHandler != null)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                serverQueryHandler.CurrentPlayers = (ushort) NetworkManager.Singleton.ConnectedClientsIds.Count;
            }

            serverQueryHandler.UpdateServerCheck();
        }
    }
}