using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.AddressableAssets;

public static class Bootstrapper
{
#if DEDICATED_SERVER
    public static GameObject DedicatedServerInstance { get; private set; }
#else    
    public static GameObject CoreSystemInstance { get; private set; }
#endif
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
#if DEDICATED_SERVER
        DedicatedServerInstance = Addressables.InstantiateAsync("Assets/Prefabs/DedicatedServer.prefab").WaitForCompletion();
        
        Object.DontDestroyOnLoad(DedicatedServerInstance);
#else
        CoreSystemInstance = Addressables.InstantiateAsync("Assets/CoreSystem.prefab").WaitForCompletion();
        
        Object.DontDestroyOnLoad(CoreSystemInstance);
#endif
    }
}
