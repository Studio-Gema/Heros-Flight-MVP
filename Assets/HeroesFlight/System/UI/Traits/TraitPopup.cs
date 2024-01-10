﻿using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.Common.Feat;
using HeroesFlight.System.FileManager.Enum;
using HeroesFlight.System.FileManager.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace HeroesFlight.System.UI.Traits
{
    public class TraitPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI DescriptionText;
        [SerializeField] private GameObject unlockBlock;
        [SerializeField] private GameObject rerollBlock;
        [SerializeField] private GameObject blockedBlock;
        [SerializeField] private Button reRollButton;
        [SerializeField] private Button unlockButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Image currencyIcon;
        [SerializeField] private TextMeshProUGUI currencyText;

        public event Action<TraitModificationEventModel> OnTraitModificationRequest;
        public event Action OnClose;
        RectTransform rect;
        private CanvasGroup canvasGroup;
        private TraitPopupState state;
        public TraitPopupState State => state;
        public TraitModel targetModel;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            reRollButton.onClick.AddListener(() =>
            {
                OnTraitModificationRequest?.Invoke(
                    new TraitModificationEventModel(targetModel, TraitModificationType.Reroll));
            });
            unlockButton.onClick.AddListener(() =>
            {
                OnTraitModificationRequest?.Invoke(
                    new TraitModificationEventModel(targetModel, TraitModificationType.Unlock));
            });
            closeButton.onClick.AddListener(HidePopup);
        }


        public void ShowPopup(Vector2 position, TraitPopupState newState, TraitModel model)
        {
            targetModel = model;
            state = newState;
            // rect.anchoredPosition3D = position;
            titleText.text = targetModel.Id;
            switch (targetModel.State)
            {
                case TraitModelState.UnlockBlocked:
                    blockedBlock.SetActive(true);
                    unlockBlock.SetActive(false);
                    rerollBlock.SetActive(false);
                    DescriptionText.text = $"Unlocks at LVL {model.RequiredLvl}";
                    break;
                case TraitModelState.UnlockPossible:
                    currencyIcon.sprite = model.TargetCurrency.GetSprite;
                    currencyText.text = model.Cost.ToString();
                    currencyText.color = model.HasEnoughCurrency ? Color.white : Color.red;
                    blockedBlock.SetActive(false);
                    unlockBlock.SetActive(true);
                    rerollBlock.SetActive(false);
                    DescriptionText.text =  ModifyDescription(model);
                    break;
                case TraitModelState.Unlocked:
                    if (model.CanBeRerolled)
                    {
                        blockedBlock.SetActive(false);
                        unlockBlock.SetActive(false);
                        rerollBlock.SetActive(true);
                    }
                    else
                    {
                        blockedBlock.SetActive(false);
                        unlockBlock.SetActive(false);
                        rerollBlock.SetActive(false);
                    }
                    DescriptionText.text =  ModifyDescription(model);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ToggleCanvasGroup(canvasGroup, true);
        }

        private string ModifyDescription(TraitModel traitModel)
        {
            var description = traitModel.Description;
            if (description.Contains("{0}"))
            {
                description = description.Replace("{0}", $"{targetModel.BaseValue}");
            }

            if (description.Contains("{1}"))
            {
                description = description.Replace("{1}", $"{targetModel.CurrentValue}");
            }

            return description;
        }

        public void UpdatePopup()
        {
        }

        public void HidePopup()
        {
            state = TraitPopupState.Disabled;
            OnClose?.Invoke();
            ToggleCanvasGroup(canvasGroup, false);
        }


        void ToggleCanvasGroup(CanvasGroup cg, bool isEnabled)
        {
            if (isEnabled)
            {
                cg.alpha = 1;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
            else
            {
                cg.alpha = 0;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }

       
    }
}