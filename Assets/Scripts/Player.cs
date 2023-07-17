using System;
using System.Collections;
using System.Collections.Generic;
using Core.Character;
using Core.Game;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class Player : Character
{
    #region PlayerSpawned

    public delegate void PlayerSpawned(Player player);

    public static event PlayerSpawned OnPlayerSpawned;

    public static List<Player> SpawnedPlayers { get; private set; } = new List<Player>();
    
    private static void InvokePlayerSpawned(Player player)
    {
        OnPlayerSpawned?.Invoke(player);
    }

    #endregion

    #region PlayerDeSpawned

    public delegate void PlayerDeSpawned(ulong ownerClientId);

    public static event PlayerDeSpawned OnPlayerDeSpawned;

    private static void InvokePlayerDeSpawned(ulong ownerClientId)
    {
        OnPlayerDeSpawned?.Invoke(ownerClientId);
    }

    #endregion
    
    #region InstanceInitialized

    public delegate void InstanceInitialized(Player instance);

    public static event InstanceInitialized OnInstanceInitialized;

    private void InvokeInstanceInitialized(Player instance)
    {
        OnInstanceInitialized?.Invoke(instance);
    }

    #endregion

    private readonly NetworkVariable<FixedString128Bytes> _networkIdLookup = new NetworkVariable<FixedString128Bytes>(string.Empty,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    private readonly NetworkVariable<FixedString128Bytes> _customIdLookup = new NetworkVariable<FixedString128Bytes>(string.Empty,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    public static Player Instance { get; private set; }

    [field: SerializeField] public bool IsNetworked { get; private set; } = true;
    
    [field: SerializeField] public bool IsLocalClient { get; private set; } = false;
    
    public string NetworkId => _networkIdLookup.Value.Value;
    
    public string CustomId => IsOwner ? UserManager.Instance.CustomId : _customIdLookup.Value.Value;
    
    private void Awake()
    {
        if (!IsLocalClient)
            return;
        
        if (Instance != null)
        {
            Debug.LogError($"more than one instance of {nameof(Player)}");
            
            return;
        }

        Instance = this;
        
        InvokeInstanceInitialized(Instance);
    }

    private void Start()
    {
        if (!IsNetworked)
        {
            _networkIdLookup.Value = UserManager.Instance.NetworkId;
            
            InitializePlayer();
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsNetworked)
            return;
        
        if (IsOwner)
        {
            _networkIdLookup.Value = UserManager.Instance.NetworkId;
            
            _customIdLookup.Value = UserManager.Instance.CustomId;
        }
        
        if (!string.IsNullOrEmpty(NetworkId) && !string.IsNullOrEmpty(CustomId))
        {
            InitializePlayer();
        }
        
        else
        {
            _networkIdLookup.OnValueChanged += (oldValue, newValue) =>
            {
                //both customId and networkId are initialized
                if (!string.IsNullOrEmpty(CustomId))
                {
                    InitializePlayer();
                }
            };
            
            _customIdLookup.OnValueChanged += (oldValue, newValue) =>
            {
                //both customId and networkId are initialized
                if (!string.IsNullOrEmpty(NetworkId))
                {
                    InitializePlayer();
                }
            };
        }

        InvokePlayerSpawned(this);
        
        SpawnedPlayers.Add(this);
        
        NetworkManager.Singleton.OnClientDisconnectCallback += InvokePlayerDeSpawned;
    }

    private void InitializePlayer()
    {
        if (GameManager.Instance.IsReady)
            Initialize();

        else
            GameManager.Instance.OnReady += Initialize;
    }
    
    public void LeaveGame()
    {
        //only owner can leave game
        if (!IsOwner)
            return;
        
        NetworkManager.Singleton.Shutdown();

        SpawnedPlayers.Clear();
        
        GameManager.Instance.ExitToMainMenu();
    }
}
