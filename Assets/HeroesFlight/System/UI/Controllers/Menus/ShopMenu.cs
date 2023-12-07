using HeroesFlight.Common.Enum;
using HeroesFlight.System.UI.Inventory_Menu;
using Pelumi.Juicer;
using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace UISystem
{
    public class ShopMenu : BaseMenu<ShopMenu>
    {
        public event Action<IAPHelper.ProductType> OnPurchaseSuccess;

        [Header("IAP Text")]
        [SerializeField] private TextMeshProUGUI gem80Text;
        [SerializeField] private TextMeshProUGUI gem500Text;
        [SerializeField] private TextMeshProUGUI gem1200Text;
        [SerializeField] private TextMeshProUGUI gem6500Text;

        [Header("IAP Buttons")]
        [SerializeField] private AdvanceButton gem80Button;
        [SerializeField] private AdvanceButton gem500Button;
        [SerializeField] private AdvanceButton gem1200Button;
        [SerializeField] private AdvanceButton gem6500Button;
        [SerializeField] private AdvanceButton restorePurchaseButton;

        [Header("Buttons")]
        [SerializeField] private AdvanceButton quitButton;

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            quitButton.onClick.AddListener(Close);

            restorePurchaseButton?.gameObject.SetActive(Application.platform == RuntimePlatform.IPhonePlayer);
        }

        public override void OnOpened()
        {
            canvasGroup.alpha = 0;
            openEffectBG.Start();
        }


        public override void OnClosed()
        {
            closeEffectBG.Start();
        }

        public override void ResetMenu()
        {
        }

        public void OnProductFetched(Product product)
        {
            switch (product.definition.id)
            {
                case IAPHelper.Gem80:
                    gem80Text.text = product.metadata.localizedPriceString + product.metadata.isoCurrencyCode;
                    break;
                case IAPHelper.Gem500:
                    gem500Text.text = product.metadata.localizedPriceString + product.metadata.isoCurrencyCode;
                    break;
                case IAPHelper.Gem1200:
                    gem1200Text.text = product.metadata.localizedPriceString + product.metadata.isoCurrencyCode;
                    break;
                case IAPHelper.Gem6500:
                    gem6500Text.text = product.metadata.localizedPriceString + product.metadata.isoCurrencyCode;
                    break;
                default: break;
            }
        }

        public void OnPurchaseComplete(Product product)
        {
            switch (product.definition.id)
            {
                case IAPHelper.Gem80:
                    OnPurchaseSuccess?.Invoke(IAPHelper.ProductType.Gem80);
                    break;
                case IAPHelper.Gem500:
                    OnPurchaseSuccess?.Invoke(IAPHelper.ProductType.Gem500);
                    break;
                case IAPHelper.Gem1200:
                    OnPurchaseSuccess?.Invoke(IAPHelper.ProductType.Gem1200);
                    break;
                case IAPHelper.Gem6500:
                    OnPurchaseSuccess?.Invoke(IAPHelper.ProductType.Gem6500);
                    break;
                default: break;
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription purchaseFailureDescription)
        {
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, purchaseFailureDescription.reason));
        }
    }
}
