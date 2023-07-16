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
    
    private void Attach(IItemData itemData)
    {
        _itemData = itemData;

        Player.Instance.GetController(out _inventoryController);
        
        AttachIcon();
        
        _displayNameLabel.text = _itemData.DisplayName;
        
        _categoryLabel.text = Utils.GetDisplayName(_itemData.Category.ToString());
    }

    public void AttachStoreItem(IItemData itemData)
    {
        Attach(itemData);
        
        _buttonText.text = "BUY";
                
        _priceLabel.text = $"{_itemData.Price}";
                
        _button.onClick.AddListener(BuyItem);
    }
    
    public void AttachInventoryItem(IItemData itemData)
    {
        Attach(itemData);
        
        _price.gameObject.SetActive(false);
                
        _buttonText.text = "EQUIP";
                
        _button.onClick.AddListener(EquipItem);
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
        _inventoryController.EquipItem(_itemData.Id);
    }

    private void BuyItem()
    {
        StoreManager.Instance.BuyItem(_itemData.Id);
    }
}
