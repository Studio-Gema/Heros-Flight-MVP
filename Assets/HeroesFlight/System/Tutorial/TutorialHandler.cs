using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Model;
using HeroesFlightProject.System.Gameplay.Controllers.ShakeProfile;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    public event Action OnPlayerEnteredPortal;

    [SerializeField] GameAreaModel tutorialModel;
    [SerializeField] BoosterDropSO mobDrop;
    [SerializeField] TutorialTrigger tutorialTrigger;
    [SerializeField] TutorialSO[] gameplayTutorial;
    [SerializeField] Reward firstItemReward;

    private LevelPortal portal;
    private Level currentLevel;

    public int CurrentLvlIndex { get; private set; }
    public bool FinishedLoop => CurrentLvlIndex >= tutorialModel.SpawnModel.Levels.Length;
    public int MaxLvlIndex => tutorialModel.SpawnModel.Levels.Length;
    public GameAreaModel GetTutorialModel => tutorialModel;
    public BoosterDropSO GetMobDrop => mobDrop;
    public TutorialTrigger GetTutorialTrigger => tutorialTrigger;
    public Level GetCurrentLevel => currentLevel;

    public TutorialSO[] GameplayTutorial => gameplayTutorial;

    public Reward FirstItemReward => firstItemReward;

    private Dictionary<TutorialMode, TutorialSO> tutorialDictionary = new Dictionary<TutorialMode, TutorialSO>();

    public void Init()
    {
        portal = Instantiate(tutorialModel.PortalPrefab, transform.position, Quaternion.identity);
        portal.gameObject.SetActive(false);
        portal.OnPlayerEntered += HandlePlayerTriggerPortal;

        for (int i = 0; i < gameplayTutorial.Length; i++)
        {
            tutorialDictionary.Add(gameplayTutorial[i].tutorialMode, gameplayTutorial[i]);
        }
    }

    private void HandlePlayerTriggerPortal()
    {
        OnPlayerEnteredPortal?.Invoke();
    }

    public Level GetLevel()
    {
        if (currentLevel != null)
        {
            return currentLevel = tutorialModel.ShrineLevel;
        }

        currentLevel = tutorialModel.SpawnModel.Levels[CurrentLvlIndex];
        CurrentLvlIndex++;

        return currentLevel;
    }

    public void EnablePortal(Vector2 position)
    {
        portal.Enable(position);
    }

    public void DisablePortal()
    {
        portal.Disable();
    }

    internal void SetStartingIndex(int v)
    {
        CurrentLvlIndex = v;
    }

    public void StartTutorialState(TutorialRuntime tutorialRuntime)
    {
        StartCoroutine(TutorialState(tutorialRuntime));
    }

    public IEnumerator TutorialState(TutorialRuntime tutorialRuntime)
    {
        tutorialRuntime.OnBegin?.Invoke();
        while (!tutorialRuntime.IsCompleted)
        {
            yield return null;
        }
        tutorialRuntime.OnEnd?.Invoke();
    }

    public TutorialSO GetTutorialSO(TutorialMode fly)
    {
        return tutorialDictionary[fly];
    }
}

public class TutorialRuntime
{
    public TutorialMode TutorialMode;
    public bool IsCompleted;
    public Action OnBegin;
    public Action OnEnd;

    public TutorialRuntime(TutorialMode tutorialMode)
    {
        TutorialMode = tutorialMode;
    }

    public void AssignEvents(Action onBegin, Action onEnd)
    {
        OnBegin = onBegin;
        OnEnd = onEnd;
    }
}