using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Pelumi.Juicer;
using System.Collections;
using StansAssets.Foundation.Async;

namespace UISystem
{
    public class GameMenu : BaseMenu<GameMenu>
    {
     //   public Func<float> GetCoinText;

        public event Action OnSingleLevelUpComplete;
        public event Action OnPauseButtonClicked;
        public event Action OnSpecialAttackButtonClicked;
        public event Action<int> OnLevelUpComplete;

        private Action OnEndTransitionHalfComplete;
        private Action OnEndTransitionComplete;

        [Header("CountDown")]
        [SerializeField] private GameObject countDownPanel;
        [SerializeField] private TextMeshProUGUI countDownText;

        [Header("Gameplay")]
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI enemyCountText;
        [SerializeField] private AdvanceButton pauseButton;

        [Header("level Progress")]
        [SerializeField] private GameObject levelProgressPanel;
        [SerializeField] private TextMeshProUGUI levelProgressText;
        [SerializeField] private Image levelProgressFill;

        [Header("Combo Counter")]
        [SerializeField] private GameObject comboCounterPanel;
        [SerializeField] private ComboFeedback[] comboFeedbacks;
        [SerializeField] private TextMeshProUGUI comboCounterText;
        [SerializeField] private TextMeshProUGUI comboFeedbackText;

        [Header("Boss")]
        [SerializeField] private GroupImageFill bossHealthFill;

        [SerializeField] GameObject bossCanvas;

        [Header("Warning")]
        [SerializeField] private GameObject warningPanel;

        [Header("Special Attack")]
        [SerializeField] private AdvanceButton specialAttackButton;
        [SerializeField] private Image specialAttackButtonFill;
        [SerializeField] private Image specialAttackIcon;

        [Header("Boosters")]
        [SerializeField] private BoosterUI[] boosterButtons;

        [Header("Transition")]
        [SerializeField] private AnimationCurve transitionCurve;
        [SerializeField] private GameObject transitionPanel;
        [SerializeField] private CanvasGroup transitionCanvasGroup;
        
        JuicerRuntime openEffect;
        JuicerRuntime closeEffect;
        JuicerRuntime countDownEffect;
        JuicerRuntime comboCounterEffect;
        JuicerRuntime comboFeedbackEffect;
        JuicerRuntime specialEffect;
        JuicerRuntime specialIconEffect;
        JuicerRuntimeCore<float> levelProgressEffect;
        JuicerRuntime transitionEffect;

        private bool isExpComplete;

        public bool IsExpComplete => isExpComplete;

        public override void OnCreated()
        {
            openEffect = canvasGroup.JuicyAlpha(1, 0.5f);
            openEffect.SetOnStart(() => canvasGroup.alpha = 0);

            closeEffect = canvasGroup.JuicyAlpha(0, 0.5f);
            closeEffect.SetOnStart(() => canvasGroup.alpha = 1);
            closeEffect.SetOnComplected(CloseMenu);

            countDownEffect = countDownText.transform.JuicyScale(1f, 0.1f);

            comboCounterEffect = comboCounterText.transform.JuicyScale(1f, 0.1f);

            comboFeedbackEffect = comboFeedbackText.transform.JuicyScale(1, 0.15f);

            specialEffect = specialAttackButtonFill.JuicyAlpha(0, 0.25f);
            specialEffect.SetEase(Ease.EaseInBounce);
            specialEffect.SetLoop( -1);

            specialIconEffect = specialAttackIcon.transform.JuicyScale(5f, 0.25f);
            specialIconEffect.SetOnComplected(() => ToggleSpecialAttackButton(false));

            pauseButton.onClick.AddListener(() => OnPauseButtonClicked?.Invoke());

            specialAttackButton.onClick.AddListener(SpecialAttackButtonClicked);

            levelProgressEffect = levelProgressFill.JuicyFillAmount(1, 1f);

            transitionEffect = transitionCanvasGroup.JuicyAlpha(1, 2f);
            transitionEffect.SetEase(transitionCurve);
            transitionEffect.SetOnStart(() => transitionPanel.gameObject.SetActive(true));
            transitionEffect.AddTimeEvent(0.5f, () =>
            {
                OnEndTransitionHalfComplete?.Invoke();
            });
            transitionEffect.SetOnComplected(() =>
            {
                OnEndTransitionComplete?.Invoke();
                transitionPanel.gameObject.SetActive(false);
            });

            ResetMenu();
        }

        public override void OnOpened()
        {
            ResetMenu();
            openEffect.Start();

           // UpdateCoinText(GetCoinText());
        }

        public override void OnClosed()
        {
            closeEffect.Start();
        }

        public override void ResetMenu()
        {
            coinText.text = "0";
            timerText.text = "00:00";
            enemyCountText.text = "0";
            comboCounterText.text = "0";
            levelProgressText.text = "LV.0";
            comboFeedbackText.text = "";

            foreach (BoosterUI boosterButton in boosterButtons)
            {
                if (boosterButton.GetBoosterSO != null)
                {
                    boosterButton.Disable();
                }
            }
        }

        public void UpateCountDownText(int value)
        {
            countDownEffect.Start(() => countDownText.transform.localScale = Vector3.zero);

            countDownPanel.gameObject.SetActive(value > 0);

            if (Mathf.CeilToInt(value) == 1)
            {
                countDownText.text = "Start!";
            }
            else
            {
                countDownText.text = Mathf.CeilToInt(value - 1).ToString();
            }
        }

        public void DisplayLevelMessage(string message, float duration = 1.5f)
        {
            countDownEffect.Start(() => countDownText.transform.localScale = Vector3.zero);
            countDownPanel.gameObject.SetActive(true);
            countDownText.text = message;
            CoroutineUtility.WaitForSeconds(duration, () => countDownPanel.gameObject.SetActive(false));
        }

        public void UpdateCoinText(float value)
        {
            coinText.JuicyTextNumber(value, 0.5f).Start();
        }

        public void UpdateTimerText(float value)
        {
            float minutes = Mathf.FloorToInt(value / 60);
            float seconds = Mathf.FloorToInt(value % 60);
            //timerText.text = (minutes == 0) ? $"{seconds.ToString("F0")}s" : $"{minutes:00}m : {seconds:00}s";
            timerText.text = (minutes == 0) ? $"{seconds.ToString("F0")}" : $"{minutes:00} : {seconds:00}";
        }

        public void UpdateTimerText(string value)
        {
            timerText.text = value;
        }

        public void UpdateEnemyCountText(int value)
        {
            enemyCountText.text = value.ToString();
        }

        public void UpdateComboCounterText(int value)
        {
            comboCounterEffect.Start(()=> comboCounterText.transform.localScale = Vector3.zero);
            comboCounterText.text = "x" + value.ToString();
            comboCounterPanel.SetActive(value != 0);

            if (value == 0)
            {
                comboFeedbackText.text = "";
                comboCounterText.text = "";
            }
            else
            {
                comboCounterPanel.SetActive(true);
                foreach (ComboFeedback comboFeedback in comboFeedbacks)
                {
                    if (comboFeedback.threshold == value)
                    {
                        comboFeedbackEffect.Start(() => comboFeedbackText.transform.localScale = Vector3.zero);
                        comboFeedbackText.text = comboFeedback.feedback;
                        break;
                    }
                }
            }
        }

        public void UpdateLevelProgressText(int value)
        {
            levelProgressText.text = value.ToString();
        }

        public void UpdateExpBar(float value)
        {
            isExpComplete = false;
            StartCoroutine(UpdateExpBarRoutine(value));
        }

        public IEnumerator UpdateExpBarRoutine(float value)
        {
            levelProgressPanel.SetActive(true);
            yield return new WaitForSeconds(0.5f);

            AudioManager.PlaySoundEffect("ExperienceUp");
            levelProgressEffect.StartNewDestination(value);

            yield return new WaitUntilJuicerComplected(levelProgressEffect);

            yield return new WaitForSeconds(0.25f);
            levelProgressPanel.SetActive(false);
            isExpComplete = true;
            yield return new WaitForSeconds(0.1f);
            isExpComplete = false;
        }

        public void UpdateExpBarLevelUp(int currentLevel, int numberOfLevelInc, float value)
        {
            isExpComplete = false;
            StartCoroutine(UpdateExpBarLevelUpRoutine(currentLevel, numberOfLevelInc, value));
        }

        public IEnumerator UpdateExpBarLevelUpRoutine(int currentLevel, int numberOfLevelInc, float value)
        {
            levelProgressPanel.SetActive(true);
            yield return new WaitForSeconds(0.5f);


            for (int i = 0; i < numberOfLevelInc; i++)
            {
                OnSingleLevelUpComplete?.Invoke();
                levelProgressEffect.StartNewDestination(1f);

                AudioManager.PlaySoundEffect("LevelUp");

                yield return new WaitUntilJuicerComplected(levelProgressEffect);

                yield return new WaitForSeconds(0.1f);

                levelProgressText.text = "LV." + (currentLevel + i + 1).ToString();
                levelProgressFill.fillAmount = 0;

                yield return new WaitForSeconds(0.1f);
            }

            if (value > 0)
            {
                AudioManager.PlaySoundEffect("ExperienceUp");
                levelProgressEffect.StartNewDestination(value);
                yield return new WaitUntilJuicerComplected(levelProgressEffect);
            }

            yield return new WaitForSeconds(1f);
            levelProgressPanel.SetActive(false);
            OnLevelUpComplete?.Invoke(currentLevel + numberOfLevelInc);
            isExpComplete = true;
            yield return new WaitForSeconds(0.1f);
            isExpComplete = false;
        }

        public void UpdateBossHealthFill(float value)
        {
            bossHealthFill.SetValue(value);
        }

        public void ShowMiniBossWarning()
        {
            warningPanel.SetActive(true);

            CoroutineUtility.WaitForSeconds(1.5f, () => warningPanel.SetActive(false));
        }

        public void ToggleBossHpBar(bool isEnabled)
        {
            bossCanvas.SetActive(isEnabled);
        }

        public void FillSpecial(float normalisedValue)
        {
            specialAttackButtonFill.fillAmount = normalisedValue;
            ToggleSpecialAttackButton(normalisedValue >= 1);
        }

        public void ToggleSpecialAttackButton(bool value)
        {
            switch (value)
            {
                case true:
                    specialAttackButtonFill.color = new Color(specialAttackButtonFill.color.r, specialAttackButtonFill.color.g, specialAttackButtonFill.color.b, 1);
                    specialEffect.Start();
                    break;
                case false:
                    specialEffect.Pause();
                    specialAttackButtonFill.color = new Color(specialAttackButtonFill.color.r, specialAttackButtonFill.color.g, specialAttackButtonFill.color.b, 1);
                    specialAttackIcon.transform.localScale = Vector3.one;
                    break;
            }
        }

        private void SpecialAttackButtonClicked()
        {
            if (specialAttackButtonFill.fillAmount < 1) return;
            
            specialAttackButtonFill.fillAmount = 0;
            specialIconEffect.Start();
            OnSpecialAttackButtonClicked?.Invoke();
        }

        public void ShowTransition(Action OntransitionHalf, Action OnEndTransition = null)
        {
            OnEndTransitionHalfComplete = OntransitionHalf;
            OnEndTransitionComplete = OnEndTransition;
            transitionEffect.Start(()=> transitionCanvasGroup.alpha = 0);
        }

        public void VisualiseBooster(BoosterContainer boosterContainer)
        {
            foreach (BoosterUI boosterButton in boosterButtons)
            {
                if (boosterButton.GetBoosterSO == null)
                {
                    boosterButton.Initialize(boosterContainer);
                    break;
                }
            }
        }
    }
}

[System.Serializable]
public class ComboFeedback
{
    public string feedback;
    public int threshold;
}