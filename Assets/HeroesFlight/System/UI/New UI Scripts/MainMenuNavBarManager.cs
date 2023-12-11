using Pelumi.Juicer;
using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuNavBarManager : MonoBehaviour
{
    public class NavigationButtonEntry
    {
        public AdvanceButton advanceButton;
        public Image buttonImage;
        public GameObject logo;
        public TextMeshProUGUI buttonText;
        public JuicerRuntime onNavButtonSelectMoveEffect;
        public JuicerRuntime onNavButtonDeselectMoveEffect;
        public JuicerRuntime onNavButtonSelectScaleEffect;
        public JuicerRuntime onNavButtonDeSelectScaleEffect;

        public NavigationButtonEntry(AdvanceButton advanceButton)
        {
            this.advanceButton = advanceButton;
            logo = advanceButton.transform.GetChild(0).gameObject.GetComponentInChildren<Image>().transform.parent.gameObject;
            buttonImage = advanceButton.GetComponent<Image>();
            buttonText = advanceButton.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    [SerializeField] public Color buttonDownColor = Color.gray;
    [SerializeField] public Color buttonUpColor = Color.white;

    private MainMenu mainMenu;

    private NavigationButtonEntry[] navigationButtons;

    private void Awake()
    {
        mainMenu = GetComponent<MainMenu>();
    }

    private void Start()
    {
        InitializeNavigationButtons(0.15f, 0.05f);
        OnNavigationButtonClick(navigationButtons[0]);
    }

    private void InitializeNavigationButtons(float selectMoveDuration, float deselectMoveDuration)
    {
        navigationButtons = new NavigationButtonEntry[mainMenu.NavigationButtons.Length];

        for (int i = 0; i < navigationButtons.Length; i++)
        {
            NavigationButtonEntry navigationButton = new NavigationButtonEntry(mainMenu.NavigationButtons[i].advanceButton);
            navigationButtons[i] = navigationButton;
            navigationButtons[i].onNavButtonDeselectMoveEffect = navigationButtons[i].logo.transform.JuicyLocalMoveY(81f, deselectMoveDuration);
            navigationButtons[i].onNavButtonSelectMoveEffect = navigationButtons[i].logo.transform.JuicyLocalMoveY((transform.position.y + 160f), selectMoveDuration);

            navigationButtons[i].onNavButtonDeSelectScaleEffect = navigationButtons[i].logo.transform.JuicyScale(Vector3.one, deselectMoveDuration);
            navigationButtons[i].onNavButtonSelectScaleEffect = navigationButtons[i].logo.transform.JuicyScale(new Vector3(1.15f, 1.15f, 1.15f), selectMoveDuration);

            navigationButtons[i].advanceButton.onClick.AddListener(() =>
            {
                OnNavigationButtonClick(navigationButton);
            });
        }
    }

    public void OnNavigationButtonClick(NavigationButtonEntry navigationBut)
    {
        foreach (NavigationButtonEntry navigationButton in navigationButtons)
        {
            navigationButton.buttonImage.color = buttonUpColor;
            navigationButton.onNavButtonDeselectMoveEffect.Start();
            navigationButton.onNavButtonDeSelectScaleEffect.Start();
            navigationButton.buttonText.gameObject.SetActive(false);
        }

        navigationBut.buttonImage.color = buttonDownColor;
        navigationBut.onNavButtonSelectMoveEffect.Start();
        navigationBut.logo.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
        navigationBut.onNavButtonSelectScaleEffect.Start();
        navigationBut.buttonText.gameObject.SetActive(true);
    }
}