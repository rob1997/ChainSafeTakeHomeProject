using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Game;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayfabUserManager : UserManager
{
    public const string TitleId = "26B00";
    
    private const string Key = "6P1FUBZ47GN7WOOJNX4CUZ1QRW1H1M9YCP9QFU1CZUSZRYBTJT";
    
    public override void Initialize()
    {
        PlayFabSettings.TitleId = TitleId;
        
        PlayFabSettings.DeveloperSecretKey = Key;

        string savedId = PlayerPrefs.GetString(nameof(CustomId), string.Empty);
        
        if (!string.IsNullOrEmpty(savedId))
        {
            Login(savedId);
        }
        
        Debug.Log("user service initialized");
    }

    public override void Login(string customId, bool keepMeSignedIn = true, LoginFailed onFailed = null)
    {
        //already logged in
        if (IsAuthenticated)
            return;
        
        Debug.Log($"logging in {customId}...");
        
        //login
        var loginRequest = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            
            CreateAccount = true
        };

        CustomId = customId;
        
        PlayFabClientAPI.LoginWithCustomID(loginRequest, LoginSuccessful, LoginFailed);
        
        void LoginFailed(PlayFabError error)
        {
            //revert value
            CustomId = string.Empty;
            
            error.LogToUnity("login with custom Id failed");
            
            onFailed?.Invoke(error.ErrorMessage);
        }
        
        void LoginSuccessful(LoginResult result)
        {
            NetworkId = result.PlayFabId;
            
            this.LoginSuccessful(keepMeSignedIn);
        }
    }
    
    public override void Logout()
    {
        if (!IsAuthenticated)
            return;
        
        Debug.Log($"Logging out {CustomId}...");
        
        PlayFabClientAPI.ForgetAllCredentials();
        
        LogoutSuccessful();
    }
}
