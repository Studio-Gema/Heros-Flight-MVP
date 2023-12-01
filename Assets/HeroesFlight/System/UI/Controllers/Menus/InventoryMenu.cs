using HeroesFlight.Common.Enum;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pelumi.Juicer;
using HeroesFlight.Common.Progression;
using HeroesFlight.System.UI.Inventory_Menu;

namespace UISystem
{
    public class InventoryMenu : BaseMenu<InventoryMenu>
    {
        public event Func<StatModel> GetStatModel;
        public event Func<CharacterSO> GetSelectedCharacterSO;
        public event Action OnChangeHeroButtonClicked;
        public event Action OnStatPointButtonClicked;
        public event Action<InventoryItemUiEntry> OnUpgradeRequest;
        public event Action<InventoryItemUiEntry> OnDismantleRequest;
        public event Action<InventoryItemUiEntry> OnEquipItemRequest;
        public event Action<InventoryItemUiEntry> OnUnEquipItemRequest;

        [Header("Buttons")] [SerializeField] private AdvanceButton changeHeroButton;
        [SerializeField] private AdvanceButton statPointButton;
        [SerializeField] private AdvanceButton quitButton;

        [Header("Texts")] [SerializeField] private TextMeshProUGUI currentAtk;
        [SerializeField] private TextMeshProUGUI currentHp;
        [SerializeField] private TextMeshProUGUI currentDef;

        [Header("Data")] [SerializeField] private UiSpineViewController uiSpineViewController;

        [Header("Inventory")] [SerializeField] private ItemUI itemUIPrefab;
        [SerializeField] private Transform itemHolder;
        [SerializeField] private ItemInfoDisplayUI itemInfoDisplayUI;
        [SerializeField] private EquippedSlot[] equippedSlots;

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        [Header("Debug")] private Dictionary<string, ItemUI> itemUIDic = new Dictionary<string, ItemUI>();
        [SerializeField] private ItemUI selectedItemUI;
        [SerializeField] private EquippedSlot selectedEquippedSlot;
        [SerializeField] InventoryItemUiEntry selectedItem;


        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            changeHeroButton.onClick.AddListener(() => OnChangeHeroButtonClicked?.Invoke());
            statPointButton.onClick.AddListener(() => OnStatPointButtonClicked?.Invoke());
            quitButton.onClick.AddListener(Close);
        }

        public override void OnOpened()
        {
            canvasGroup.alpha = 0;
            openEffectBG.Start();
            UpdateCharacter(GetSelectedCharacterSO.Invoke());
            //LoadInventoryItems();
        }

        public void UpdateCharacter(CharacterSO characterSO)
        {
            uiSpineViewController.SetupView(characterSO);
            OnStatValueChanged(GetStatModel.Invoke());
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
        }

        public override void ResetMenu()
        {
        }

        public void OnStatValueChanged(StatModel statModel)
        {
            foreach (var stat in statModel.CurrentStatDic)
            {
                switch (stat.Key)
                {
                    case StatType.PhysicalDamage:
                        currentAtk.text = stat.Value.ToString("F0");
                        break;
                    case StatType.MaxHealth:
                        currentHp.text = stat.Value.ToString("F0");
                        break;
                    case StatType.Defense:
                        currentDef.text = stat.Value.ToString("F0");
                        break;
                }
            }
        }

        public void InitInventory(Func<EquipmentEntryUi, int> maxLvlCallback, Func<EquipmentEntryUi, int> goldcallback,
            Func<string, InventoryItemUiEntry> getMaterialCallback, Func<string, InventoryItemUiEntry> getitemCallback,
            Func<EquipmentEntryUi, int> materialAmountCallback, Func<EquipmentEntryUi, int> materialSpent,
            Func<EquipmentEntryUi, int> goldSpent)
        {
            itemInfoDisplayUI.OnEquipAction += EquipItem;
            itemInfoDisplayUI.OnDismantleAction += () =>
            {
                OnDismantleRequest?.Invoke(selectedItem);
                //todo :Move to inventory system
                // DismantleItem;
            };
            itemInfoDisplayUI.OnUpgradeRequest += () => { OnUpgradeRequest?.Invoke(selectedItem); };
            itemInfoDisplayUI.OnUpgradeAction += TryUpgradeItem;
           
            itemInfoDisplayUI.OnUnequipAction += UnEquipItem;

            itemInfoDisplayUI.Init(maxLvlCallback, goldcallback,
                getMaterialCallback, getitemCallback,
                materialAmountCallback, materialSpent,
                goldSpent);

            foreach (EquippedSlot slot in equippedSlots)
            {
                slot.OnSelectItem += (item) =>
                {
                    selectedEquippedSlot = slot;
                    ItemSelected(item);
                };
            }
        }

        public void ClearInventoryItems()
        {
            foreach (ItemUI itemUI in itemUIDic.Values)
            {
                Destroy(itemUI.gameObject);
            }

            itemUIDic.Clear();
        }

        public void LoadInventoryItems(List<EquipmentEntryUi> equipementItems, List<InventoryItemUiEntry> materials)
        {
            ClearInventoryItems();

            foreach (var item in equipementItems)
            {
                if (item.IsEquipped)
                {
                    EquippedSlot equippedSlot = GetEquipmentSlot(item.EquipmentType);
                    if (equippedSlot != null)
                    {
                        equippedSlot.Occupy(item, item.RarityPallete);
                    }
                }
                else
                {
                    SpawnItemUI(item);
                }
            }

            foreach (var item in materials)
            {
                SpawnItemUI(item);
            }
        }

        /// <summary>
        /// Inventory systemn should do it
        /// </summary>
        /// <returns></returns>
        public void TryUpgradeItem()
        {
            if (selectedItem != null)
            {
                OnUpgradeRequest?.Invoke(selectedItem);
            }
        }

        public void UpgradeItem()
        {
            if (selectedItem != null)
            {
                itemInfoDisplayUI.UpgradeItem();
            }
           
        }

        /// <summary>
        /// Inventory systemn should do it
        /// </summary>
        /// <returns></returns>
        public void DismantleItem()
        {
            if (selectedItem != null)
            {
                itemUIDic.Remove(selectedItem.ID);
            }

            if (selectedItemUI != null)
            {
                Destroy(selectedItemUI.gameObject);
                selectedItemUI = null;
            }

            if (selectedEquippedSlot != null)
            {
                selectedEquippedSlot.UnOccupy();
                selectedItemUI = null;
            }

            itemInfoDisplayUI.Close();
        }

        /// <summary>
        /// Inventory systemn should do it
        /// </summary>
        /// <returns></returns>
        public void EquipItem()
        {
            EquippedSlot equippedSlot = GetEquipmentSlot((selectedItemUI.GetItem as EquipmentEntryUi).EquipmentType);
            if (equippedSlot != null)
            {
                if (equippedSlot.IsOccupied)
                {
                    OnUnEquipItemRequest?.Invoke(equippedSlot.GetItem);
                    SpawnItemUI(equippedSlot.GetItem);
                    equippedSlot.UnOccupy();
                }

                OnEquipItemRequest?.Invoke(equippedSlot.GetItem);
                equippedSlot.Occupy(selectedItemUI.GetItem, selectedItemUI.GetItem.RarityPallete);
                itemUIDic.Remove(selectedItemUI.GetItem.ID);
                Destroy(selectedItemUI.gameObject);
                selectedItemUI = null;
            }

            itemInfoDisplayUI.Close();
        }

        /// <summary>
        /// Inventory systemn should do it
        /// </summary>
        /// <returns></returns>
        public void UnEquipItem()
        {
            if (selectedEquippedSlot != null)
            {
                OnUnEquipItemRequest?.Invoke(selectedEquippedSlot.GetItem);
                SpawnItemUI(selectedEquippedSlot.GetItem);
                selectedEquippedSlot.UnOccupy();
                selectedEquippedSlot = null;
            }

            itemInfoDisplayUI.Close();
        }

        public void SpawnItemUI(List<InventoryItemUiEntry> items)
        {
            foreach (InventoryItemUiEntry item in items)
            {
                SpawnItemUI(item);
            }
        }

        public void SpawnItemUI(InventoryItemUiEntry item)
        {
            ItemUI itemUI = Instantiate(itemUIPrefab, itemHolder);
            string id = "";
            switch (item.ItemType)
            {
                case ItemType.Equipment:
                    itemUI.SetItem(item, item.RarityPallete);
                    id = item.ID;
                    break;
                case ItemType.Material:
                    itemUI.SetItem(item, item.RarityPallete);
                    id = item.ID;
                    break;
                default:
                    break;
            }

            itemUI.OnSelectItem += (itemUI) =>
            {
                selectedItemUI = itemUI;
                ItemSelected(itemUI.GetItem);
            };

            itemUIDic.Add(id, itemUI);
        }

        private void ItemSelected(InventoryItemUiEntry item)
        {
            switch (item.ItemType)
            {
                case ItemType.Equipment:
                    itemInfoDisplayUI.Display(item as EquipmentEntryUi);
                    selectedItem = item;
                    break;
                case ItemType.Material:
                    break;
            }
        }

        public void UpdateItemUI(InventoryItemUiEntry item)
        {
            string id = "";
            switch (item.ItemType)
            {
                case ItemType.Equipment:
                    id = item.ID;
                    break;
                case ItemType.Material:
                    id = item.ID;
                    break;
            }

            if (itemUIDic.ContainsKey(id))
            {
                itemUIDic[id].SetItemInfo();
            }
            else
            {
                GetEquippedSlot(item)?.SetItemInfo();
            }
        }

        private EquippedSlot GetEquippedSlot(InventoryItemUiEntry item)
        {
            foreach (EquippedSlot slot in equippedSlots)
            {
                if (slot.GetItem == item)
                {
                    return slot;
                }
            }

            return null;
        }

        private EquippedSlot GetEquipmentSlot(EquipmentType equipmentType)
        {
            int duplicateCount = 0;
            foreach (EquippedSlot slot in equippedSlots)
            {
                if (slot.GetEquipmentType == equipmentType)
                {
                    if (equipmentType == EquipmentType.Ring)
                    {
                        if (!slot.IsOccupied || duplicateCount == 1)
                        {
                            return slot;
                        }
                        else
                        {
                            duplicateCount++;
                        }
                    }
                    else
                    {
                        return slot;
                    }
                }
            }

            return null;
        }
    }
}