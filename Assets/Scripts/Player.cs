using System;
using System.Collections;
using System.Collections.Generic;
using Core.Character;
using UnityEngine;

public class Player : Character
{
    #region InstanceInitialized

    public delegate void InstanceInitialized(Player instance);

    public static event InstanceInitialized OnInstanceInitialized;

    private void InvokeInstanceInitialized(Player instance)
    {
        OnInstanceInitialized?.Invoke(instance);
    }

    #endregion
    
    public static Player Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"more than one instance of {nameof(Player)}");
            
            return;
        }

        Instance = this;
        
        InvokeInstanceInitialized(Instance);
    }
}
