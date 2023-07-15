using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Game;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayfabUserManager : Manager<PlayfabUserManager>
{
    public const string TitleId = "26B00";
    
    private const string Key = "6P1FUBZ47GN7WOOJNX4CUZ1QRW1H1M9YCP9QFU1CZUSZRYBTJT";

    public static string CustomId;

    #region Authenticated

    public delegate void Authenticated();

    public event Authenticated OnAuthenticated;

    private void InvokeAuthenticated()
    {
        OnAuthenticated?.Invoke();
    }

    #endregion
    
    public bool IsAuthenticated { get; private set; }
    
    public override void Initialize()
    {
        PlayFabSettings.TitleId = TitleId;
        
        PlayFabSettings.DeveloperSecretKey = Key;

        string savedId = PlayerPrefs.GetString(nameof(CustomId), string.Empty);
        
        if (!string.IsNullOrEmpty(savedId))
        {
            LoginAnonymously(savedId);
        }
        
        Debug.Log("user service initialized");
    }

    public void Logout()
    {
        if (!IsAuthenticated)
        {
            return;
        }
        
        Debug.Log($"Logging out {CustomId}...");
        
        PlayFabClientAPI.ForgetAllCredentials();

        IsAuthenticated = false;
        
        PlayerPrefs.SetString(nameof(CustomId), string.Empty);
        
        PlayerPrefs.Save();
        
        GameManager.Instance.ResetGame();
    }

    private void LoginSuccessful(bool keepMeSignedIn)
    {
        if (keepMeSignedIn)
        {
            PlayerPrefs.SetString(nameof(CustomId), CustomId);
            
            PlayerPrefs.Save();
        }

        IsAuthenticated = true;
        
        InvokeAuthenticated();
        
        GameManager.Instance.FinishedLoading();
        
        Debug.Log($"{CustomId} logged in successfully");
    }

    public void LoginAnonymously(string customId, bool keepMeSignedIn = true, Action<string> onError = null)
    {
        //login
        var loginRequest = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            
            CreateAccount = true
        };

        CustomId = customId;
        
        PlayFabClientAPI.LoginWithCustomID(loginRequest, result => { LoginSuccessful(keepMeSignedIn); }, LoginFailed);
        
        void LoginFailed(PlayFabError error)
        {
            LogFailedRequest("login with custom Id failed", error); 
                
            onError?.Invoke(error.ErrorMessage);
        }
    }
    
    public static void LogFailedRequest(string suffix, PlayFabError error)
    {
        Debug.LogError($"{suffix}, {error.GenerateErrorReport()}");
    }
}
