using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.AddressableAssets;

public static class Bootstrapper
{
    public static GameObject CoreSystemInstance { get; private set; }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        CoreSystemInstance = Addressables.InstantiateAsync("Assets/CoreSystem.prefab").WaitForCompletion();
        
        Object.DontDestroyOnLoad(CoreSystemInstance);
    }
}
