using System;
using System.Collections;
using System.Collections.Generic;
using Core.Game;
using UnityEngine;

public delegate void LoginFailed(string errorMessage);

public abstract class UserManager : Manager<UserManager>
{
    public string CustomId { get; protected set; }
    
    #region Authenticated

    public delegate void Authenticated();

    public event Authenticated OnAuthenticated;

    private void InvokeAuthenticated()
    {
        IsAuthenticated = true;
        
        OnAuthenticated?.Invoke();
        
        Debug.Log($"{CustomId} authenticated");
    }

    #endregion
    
    public bool IsAuthenticated { get; private set; }
    
    public abstract void Login(string customId, bool keepMeSignedIn = true, LoginFailed onFailed = null);

    protected void LoginSuccessful(bool keepMeSignedIn)
    {
        if (keepMeSignedIn)
        {
            PlayerPrefs.SetString(nameof(CustomId), CustomId);
            
            PlayerPrefs.Save();
        }

        InvokeAuthenticated();
        
        GameManager.Instance.FinishedLoading();
        
        Debug.Log($"{CustomId} logged in successfully");
    }
    
    public abstract void Logout();

    protected void LogoutSuccessful()
    {
        IsAuthenticated = false;
        
        PlayerPrefs.SetString(nameof(CustomId), string.Empty);
        
        PlayerPrefs.Save();
        
        GameManager.Instance.ResetGame();
    }
}
