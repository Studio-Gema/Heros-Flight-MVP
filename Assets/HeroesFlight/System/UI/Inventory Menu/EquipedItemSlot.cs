using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquippedSlot : MonoBehaviour
{
    public Action<Item> OnSelectItem;

    [SerializeField] private EquipmentType equipmentType;
    [SerializeField] AdvanceButton selectButton;
    [SerializeField] GameObject content;
    [SerializeField] Image itemRarityColour;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemInfo;

    private bool isOccupied;
    private Item itemInSlot;

    public EquipmentType GetEquipmentType => equipmentType;
    public bool IsOccupied => isOccupied;
    public Item GetItem => itemInSlot;

    private void Start()
    {
        selectButton.onClick.AddListener(SelectItem);
    }

    public void Occupy(Item item, RarityPalette rarityPalette)
    {
        content.SetActive(true);
        isOccupied = true;
        itemInSlot = item;
        itemIcon.sprite = GetItem.itemSO.icon;
        itemRarityColour.color = rarityPalette.backgroundColour;
        SetItemInfo();
    }

    public void SetItemInfo()
    {
        itemInfo.text = "LV." + itemInSlot.GetItemData<ItemEquipmentData>().value.ToString();
    }

    private void SelectItem()
    {
        if (isOccupied)
        {
            OnSelectItem?.Invoke(itemInSlot);
        }
    }

    public void UnOccupy()
    {
        isOccupied = false;
        itemInSlot = null;
        content.SetActive(false);
    }
}
