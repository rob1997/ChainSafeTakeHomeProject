using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct SlotUi
{
    [field: SerializeField] public Image Image { get; private set; }
    
    [field: SerializeField] public Button UnEquipButton { get; private set; }
}

public class SlotsUiAdapter : MonoBehaviour
{
    [SerializeField] private Button _leaveButton;
    
    [SerializeField] private TMP_Text _idLabel;
    
    [HideInInspector] public GenericDictionary<ItemCategory, SlotUi> SlotsUiLookup = GenericDictionary<ItemCategory, SlotUi>
        .ToGenericDictionary(Utils.GetEnumValues<ItemCategory>().ToDictionary(c => c, c => default(SlotUi)));
    
    private Player _player;
    
    private InventoryController _inventoryController;
    
    public void Attach(Player player)
    {
        _player = player;
        
        _player.GetController(out _inventoryController);
        
        if (_inventoryController.IsInventoryInitialized)
            Initialize();

        else
            _inventoryController.OnInventoryInitialized += Initialize;
        
        Initialize();
    }
    
    private void Initialize()
    {
        //initialize slots
        foreach (var pair in _inventoryController.Bag.Slots)
        {
            if (!string.IsNullOrEmpty(pair.Value))
            {
                EquipUiItem(pair.Value);
            }
        }
        
        _inventoryController.Bag.OnItemEquipped += EquipUiItem;
        
        _inventoryController.Bag.OnSlotUnEquipped += UnEquipUiSlot;
        
        foreach (var pair in SlotsUiLookup)
        {
            pair.Value.UnEquipButton.onClick.AddListener(delegate { UnEquipSlot(pair.Key); });
        }

        if (_idLabel != null)
            _idLabel.text = _player.CustomId;

        if (_leaveButton != null && _player.IsOwner)
        {
            _leaveButton.gameObject.SetActive(true);
                
            _leaveButton.onClick.AddListener(_player.LeaveGame);
        }
    }
    
    private void EquipUiItem(string itemId)
    {
        var itemData = _inventoryController.Bag.AllItems.FirstOrDefault(i => i.Id == itemId);
        
        EquipUiItem(itemData);
    }
    
    private void EquipUiItem(IItemData itemData)
    {
        Utils.LoadAsset<Texture2D>(itemData.SpriteAssetPath, result =>
        {
            SlotsUiLookup[itemData.Category].Image.sprite = Sprite.Create(result, new Rect(0f, 0f, result.width, result.height), Vector2.zero);
        });
    }
    
    private void UnEquipUiSlot(ItemCategory category)
    {
        SlotsUiLookup[category].Image.sprite = null;
    }
    
    private void UnEquipSlot(ItemCategory category)
    {
        _inventoryController.UnEquipSlot(category);
    }
}
