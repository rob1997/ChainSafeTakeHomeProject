using System.Collections;
using System.Collections.Generic;
using Core.Game;
using TMPro;
using Ui.Main;
using UnityEngine;
using UnityEngine.UI;

public class LoginUiMenu : UiMenu
{
    [SerializeField] private TMP_InputField _customIdInputField;
    
    [SerializeField] private Toggle _keepMeSignedInToggle;
    
    [SerializeField] private Button _loginButton;
    
    [SerializeField] private TMP_Text _errorLabel;

    public override void Initialize(UiRegion rootUiElement)
    {
        base.Initialize(rootUiElement);

        OnUiMenuStateChanged += state =>
        {
            if (state == UiMenuState.Loaded)
            {
                if (PlayfabUserManager.Instance.IsAuthenticated)
                    LoginSuccessful();

                else
                    PlayfabUserManager.Instance.OnAuthenticated += LoginSuccessful;
            }
        };
        
        _loginButton.onClick.AddListener(Login);
    }

    private void Login()
    {
        string customId = _customIdInputField.text;

        bool keepMeSignedIn = _keepMeSignedInToggle.isOn;
        
        PlayfabUserManager.Instance.LoginAnonymously(customId, keepMeSignedIn, LoginFailed);
    }

    private void LoginSuccessful()
    {
        UiRegion.TryQueueActiveUiMenuUnload();
    }
    
    private void LoginFailed(string message)
    {
        _errorLabel.text = message;
    }
}
