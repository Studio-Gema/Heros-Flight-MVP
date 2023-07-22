using System;
using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

public class UIEventHandler : MonoBehaviour
{
    [SerializeField] private UIManager uIManager;
    [SerializeField] private ConfirmationUISO backToMenu;

    private MainMenu mainMenu = null;
    private SettingsMenu settingsMenu = null;
    private LoadingMenu loadingMenu = null;
    private GameMenu gameMenu = null;
    private PauseMenu pauseMenu = null;
    private ConfirmationMenu confirmationMenu = null;
    private CountDownTimer startTimer;
    private ReviveMenu reviveMenu = null;
    private SummaryMenu summaryMenu = null;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        OnGameOpened();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            reviveMenu.Open();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            summaryMenu.Open();
        }
    }

    private void Init()
    {
        loadingMenu = uIManager.InitMenu<LoadingMenu>();
        confirmationMenu = uIManager.InitMenu<ConfirmationMenu>();

        mainMenu = uIManager.InitMenu<MainMenu>();
        mainMenu.OnMenuOpened += () =>
        {
            AudioManager.PlayMusic("MainMenu");
        };

        mainMenu.OnPlayButtonPressed += OnPlayButtonPressed;
        mainMenu.OnSettingsButtonPressed += () =>
        {
            settingsMenu.Open();
        };

        settingsMenu = uIManager.InitMenu<SettingsMenu>();
        settingsMenu.OnBackButtonPressed += () =>
        {
            settingsMenu.Close();
        };

        gameMenu = uIManager.InitMenu<GameMenu>();
        gameMenu.OnPauseButtonClicked += () =>
        {
            pauseMenu.Open();
        };

        pauseMenu = uIManager.InitMenu<PauseMenu>();
        pauseMenu.OnSettingsButtonClicked += () =>
        {
            settingsMenu.Open();
        };

        pauseMenu.OnResumeButtonClicked += () =>
        {
            pauseMenu.Close();
        };

        pauseMenu.OnQuitButtonClicked += () =>
        {
            confirmationMenu.Display(backToMenu, ReturnToMainMenu, null);
        };

        reviveMenu = uIManager.InitMenu<ReviveMenu>();
        reviveMenu.OnWatchAdsButtonClicked += () =>
        {
            return true;       
        };

        reviveMenu.OnGemButtonClicked += () =>
        {
            return true;
        };

        summaryMenu = uIManager.InitMenu<SummaryMenu>();
        summaryMenu.OnContinueButtonClicked += () =>
        {

        };

        startTimer = new CountDownTimer(this);
    }

    public void OnGameOpened()
    {
        mainMenu.Open();
    }

    private void OnPlayButtonPressed()
    {
        mainMenu.Close();
        gameMenu.Open();

        startTimer.Start(3, (time) =>
        {
            gameMenu.UpdateTimerText(time);
        }, 
        () =>
        {
            startTimer.Start(200, (time) =>
            {
                gameMenu.UpdateTimerText(time);
            },
          () =>
          {
              Debug.Log("Game Time Lapse");
          });
        });

        AudioManager.BlendTwoMusic("ForestStart", "ForestLoop");
    }

    public void ReturnToMainMenu()
    {
        gameMenu.Close();
        pauseMenu.Close();
        mainMenu.Open();
    }
}
