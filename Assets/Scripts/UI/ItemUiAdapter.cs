using System.Collections;
using System.Collections.Generic;
using Core.Utils;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUiAdapter : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    
    [SerializeField] private TMP_Text _displayNameLabel;
    
    [SerializeField] private TMP_Text _categoryLabel;
    
    [SerializeField] private Transform _price;
    
    [SerializeField] private TMP_Text _priceLabel;

    [SerializeField] private Button _button;
    
    [SerializeField] private TMP_Text _buttonText;

    private IItemData _itemData;
    
    private InventoryController _inventoryController;
    
    public void Attach(IItemData itemData, bool isStoreDisplay)
    {
        _itemData = itemData;

        AttachIcon();
        
        _displayNameLabel.text = _itemData.DisplayName;
        
        _categoryLabel.text = Utils.GetDisplayName(_itemData.Category.ToString());

        if (isStoreDisplay)
        {
            _buttonText.text = "BUY";
                
            _priceLabel.text = $"{_itemData.Price}";
                
            _button.onClick.AddListener(BuyItem);
        }

        else
        {
            _price.gameObject.SetActive(false);
                
            _buttonText.text = "Equip";
                
            _button.onClick.AddListener(EquipItem);
        }
    }

    private void AttachIcon()
    {
        Utils.LoadAsset<Texture2D>(_itemData.SpriteAssetPath, result =>
        {
            _itemIcon.sprite = Sprite.Create(result, new Rect(0f, 0f, result.width, result.height), Vector2.zero);
        });
    }

    private void EquipItem()
    {
        if (_inventoryController == null)
            Player.Instance.GetController(out _inventoryController);

        _inventoryController.EquipItem(_itemData.Id);
    }

    private void BuyItem()
    {
        StoreManager.Instance.BuyItem(_itemData.Id);
    }
}
