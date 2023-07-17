using System.Collections;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;

public static class PlayfabUtils
{
    public const string CoinCurrencyKey = "CN";
    
    public const string GetUserDataCloudFunctionName = "getUserData";
    
    public const string GetUserInventoryCloudFunctionName = "getUserInventory";
    
    public static void LogToUnity(this PlayFabError error, string suffix)
    {
        Debug.LogError($"{suffix}, {error.GenerateErrorReport()}");
    }
}
