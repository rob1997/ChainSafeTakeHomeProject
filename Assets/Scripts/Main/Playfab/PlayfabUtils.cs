using System.Collections;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;

public static class PlayfabUtils
{
    public const string CoinCurrencyKey = "CN";
    
    public static void LogToUnity(this PlayFabError error, string suffix)
    {
        Debug.LogError($"{suffix}, {error.GenerateErrorReport()}");
    }
}
