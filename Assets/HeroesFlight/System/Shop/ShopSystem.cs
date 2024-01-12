using HeroesFlight.System.Inventory;
using HeroesFlight.System.UI;
using StansAssets.Foundation.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

public class ShopSystem : IShopSystemInterface
{
    public ShopDataHolder ShopDataHolder { get; private set; }
    private IUISystem uISystem;
    private RewardSystemInterface rewardSystem;
    private InventorySystemInterface inventorySystem;
    private DataSystemInterface dataSystem; 

    public ShopSystem(IUISystem uISystemInterface, RewardSystemInterface rewardSystemInterface, InventorySystemInterface inventorySystemInterface, DataSystemInterface dataSystemInterface)
    {
        uISystem = uISystemInterface;
        rewardSystem = rewardSystemInterface;
        inventorySystem = inventorySystemInterface;
        dataSystem = dataSystemInterface;
    }

    public void Init(Scene scene = default, Action onComplete = null)
    {
        ShopDataHolder = scene.GetComponent<ShopDataHolder>();
    }


    public void InjectUiConnection()
    {
        // Suscribe to UI events
        uISystem.UiEventHandler.ShopMenu.OnPurchaseSuccess += OnPurchaseSuccess;

        uISystem.UiEventHandler.ShopMenu.TryPurchaseChest += (chestType) =>
        {
            Chest chest = ShopDataHolder.GetChest(chestType);
           
            uISystem.UiEventHandler.ConfirmationMenu.Display("Buy Chest"
                , "Are you sure you want to buy this chest for " +
                ColoredText(chest.GetGemChestPrice.ToString(), Color.yellow) + " gems?"
                , chest.GetChestInfo
                , () => BuyChestWithGem(chestType), null);
        };

        uISystem.UiEventHandler.ShopMenu.TryPurchaseGoldPack += (goldPackType) =>
        {
            GoldPackGroup pack = ShopDataHolder.GetGoldPack();
            uISystem.UiEventHandler.ConfirmationMenu.Display("Buy Gold Pack"
                , "Are you sure you want to buy " + ColoredText(pack.GetContent((int)goldPackType).reward.GetAmount().ToString(),Color.yellow) 
                + " Gold for " +
                ColoredText(pack.GetContent((int)goldPackType).cost.ToString(), Color.yellow) + " gems?"
               ,""
                , () => BuyGoldPack(goldPackType), null);
        };

        foreach (GoldPackGroup.GoldPack goldPack in ShopDataHolder.GoldPackGroup.GetGoldPacks)
        {
            uISystem.UiEventHandler.ShopMenu.SetGoldPackInfo(goldPack.goldPackType, goldPack.reward.GetAmount(),  goldPack.cost);
        }

        foreach (Chest chest in ShopDataHolder.GetChests)
        {
            uISystem.UiEventHandler.ShopMenu.SetChestInfo(chest.GetChestType, chest.GetChestInfo, chest.GetGemChestPrice);
        }
    }

    private void OnPurchaseSuccess(IAPHelper.ProductType obj)
    {
        switch (obj)
        {
            case IAPHelper.ProductType.Gem80:
                dataSystem.CurrencyManager.AddCurrency(CurrencyKeys.Gem, 80);
                break;
            case IAPHelper.ProductType.Gem500:
                dataSystem.CurrencyManager.AddCurrency(CurrencyKeys.Gem, 500);
                break;
            case IAPHelper.ProductType.Gem1200:
                dataSystem.CurrencyManager.AddCurrency(CurrencyKeys.Gem, 1200);
                break;
            case IAPHelper.ProductType.Gem6500:
                dataSystem.CurrencyManager.AddCurrency(CurrencyKeys.Gem, 6500);
                break;
            default: break;
        }
    }

    public void Reset()
    {
        // Reset UI events
    }

    public void ProccessRewards(List<Reward> rewards)
    {
        rewardSystem.ProcessRewards(rewards);
    }

    public void BuyChestWithAds(ChestType chestType)
    {
        Chest chest = ShopDataHolder.GetChest(chestType);
        if (chest == null)
            return;

        if (dataSystem.CurrencyManager.GetCurrencyAmount(CurrencyKeys.Gold) >= chest.GetGemChestPrice)
        {
            //Debug.Log("Chest bought");
            //dataSystem.CurrencyManager.ReduceCurency(CurrencyKeys.Gold, chest.GetGemChestPrice);
            //rewardSystem.ProcessRewards(chest.OpenChest());

            //      uISystem.UiEventHandler.

            dataSystem.AdManager.ShowRewarededAd(() =>
            {
                List<Reward> rewards = chest.OpenChest();
                rewardSystem.ProcessRewards(rewards);
                uISystem.UiEventHandler.RewardMenu.DisplayRewardsVisual(chestType, rewardSystem.GetRewardVisuals(rewards).ToArray());
            });
        }
        else
        {
            Debug.Log("Not enough gold");
        }
    }

    public void BuyChestWithGem(ChestType chestType)
    {
        Chest chest = ShopDataHolder.GetChest(chestType);
        if (chest == null)
            return;

        if (dataSystem.CurrencyManager.GetCurrencyAmount(CurrencyKeys.Gem) >= chest.GetGemChestPrice)
        {
            dataSystem.CurrencyManager.ReduceCurency(CurrencyKeys.Gem, chest.GetGemChestPrice);
            List<Reward> rewards = chest.OpenChest();
            rewardSystem.ProcessRewards(rewards);
            uISystem.UiEventHandler.RewardMenu.DisplayRewardsVisual(chestType, rewardSystem.GetRewardVisuals(rewards).ToArray());
        }
        else
        {
            Debug.Log("Not enough gems");
        }
    }

    public void BuyGoldPack(GoldPackType goldPack)
    {
        int index = (int)goldPack;
        GoldPackGroup pack = ShopDataHolder.GetGoldPack();
        if (pack == null) 
            return;

        GoldPackGroup.GoldPack content = pack.GetContent(index);

        if (goldPack == GoldPackType.Small)
        {
            // use ads
            dataSystem.AdManager.ShowRewarededAd(() =>
            {
                dataSystem.CurrencyManager.AddCurrency(content.reward.GetRewardObject<CurrencySO>(), content.reward.GetAmount());
                uISystem.UiEventHandler.RewardMenu.DisplayRewardsVisual(rewardSystem.GetRewardVisual(content.reward));
            });
        }
        else
        {
            if (dataSystem.CurrencyManager.GetCurrencyAmount(CurrencyKeys.Gem) >= content.cost)
            {
                dataSystem.CurrencyManager.ReduceCurency(CurrencyKeys.Gem, content.cost);
                dataSystem.CurrencyManager.AddCurrency(content.reward.GetRewardObject<CurrencySO>(), content.reward.GetAmount());
                uISystem.UiEventHandler.RewardMenu.DisplayRewardsVisual(rewardSystem.GetRewardVisual(content.reward));
            }
            else
            {
                Debug.Log("Not enough gems");
            }
        }
    }

    public string ColoredText(string text, Color color)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
    }
}
