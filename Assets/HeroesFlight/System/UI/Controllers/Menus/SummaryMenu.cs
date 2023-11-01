using Pelumi.Juicer;
using UnityEngine;
using TMPro;
using System;
using UISystem.Entries;

namespace UISystem
{
    public class SummaryMenu : BaseMenu<SummaryMenu>
    {
        public Func<string> GetCurrentGold;

        public event Action OnContinueButtonClicked;

        [Header("Texts")]
        [SerializeField] TextMeshProUGUI coinText;
        [SerializeField] TextMeshProUGUI killsText;
        [SerializeField] TextMeshProUGUI timeText;

        [Header("Buttons")]
        [SerializeField] AdvanceButton homeButton;
        [SerializeField] AdvanceButton retryButton;
        [SerializeField] AdvanceButton continueButton;


        [SerializeField] RewardEntry entryPrefab;
        [SerializeField] Transform rewardsParent;
        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            continueButton.onClick.AddListener(CloseButtonAction);
            continueButton.onClick.AddListener(CloseButtonAction);
            continueButton.onClick.AddListener(CloseButtonAction);
        }

        public override void OnOpened()
        {
            openEffectBG.Start();

            if (GetCurrentGold != null)
                coinText.text = GetCurrentGold.Invoke();
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
            foreach (Transform child in rewardsParent)
            {
                Destroy(child.gameObject);
            }
        }

        public override void ResetMenu()
        {

        }

        private void CloseButtonAction()
        {
            OnContinueButtonClicked?.Invoke();
            Close();
        }

        public void AddRewardEntry(string summary)
        {
            var entry = Instantiate(entryPrefab, rewardsParent);
            entry.SetupReward(summary);
        }
    }
}