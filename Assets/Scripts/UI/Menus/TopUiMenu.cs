using System.Collections;
using System.Collections.Generic;
using TMPro;
using Ui.Main;
using UnityEngine;
using UnityEngine.UI;

public class TopUiMenu : UiMenu
{
    [SerializeField] private TMP_Text _customIdLabel;
    
    [SerializeField] private TMP_Text _currencyLabel;
    
    [SerializeField] private Button _logoutButton;

    private int _currencyCopy;
    
    public override void Initialize(UiRegion rootUiElement)
    {
        base.Initialize(rootUiElement);

        _customIdLabel.text = PlayfabUserManager.Instance.CustomId;
        
        _logoutButton.onClick.AddListener(delegate { PlayfabUserManager.Instance.Logout(); });

        //initialize and set currency
        Player.Instance.GetController(out InventoryController inventoryController);

        if (inventoryController.IsInventoryInitialized)
            InitializeCurrency();
        
        else
            inventoryController.OnInventoryInitialized += InitializeCurrency;
        
        void InitializeCurrency()
        {
            _currencyCopy = inventoryController.Bag.Currency;
            
            SetCurrency();
            
            inventoryController.Bag.OnCurrencyUpdated += change =>
            {
                _currencyCopy += change;
                
                SetCurrency();
            };
        }
    }
    
    private void SetCurrency()
    {
        _currencyLabel.text = $"{_currencyCopy}";
    }
}
