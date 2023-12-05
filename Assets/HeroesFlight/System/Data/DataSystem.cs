using StansAssets.Foundation.Extensions;
using System;

using HeroesFlight.System.FileManager.Rewards;
using UnityEngine.SceneManagement;

public class DataSystem : DataSystemInterface
{
    public DataSystem()
    {
        RewardHandler = new RewardsHandler();
    }

    public RewardsHandlerInterface RewardHandler { get; private set; }
    public event Action OnApplicationQuit;
    public void RequestDataSave()
    {
        OnApplicationQuit?.Invoke();
    }

    public CharacterManager CharacterManager { get; private set; }

    public CurrencyManager CurrencyManager { get; private set; }

    public StatManager StatManager { get; private set; }

    public StatPoints StatPoints { get; private set; }

    public AccountLevelManager AccountLevelManager { get; private set; }
 

    public WorldManager WorldManger { get; private set; }


    public void Init(Scene scene = default, Action onComplete = null)
    {
        CurrencyManager = scene.GetComponent<CurrencyManager>();
        StatManager = scene.GetComponent<StatManager>();
        StatPoints = scene.GetComponent<StatPoints>();
        CharacterManager = scene.GetComponent<CharacterManager>();
        AccountLevelManager = scene.GetComponent<AccountLevelManager>();
        WorldManger = scene.GetComponent<WorldManager>();

        CurrencyManager.LoadCurrencies();

        StatManager.Init();

        StatPoints.OnValueChanged += StatManager.ProcessStatPointsModifiers;
        StatPoints.Init();

        CharacterManager.OnCharacterChanged += (characterSO) => StatManager.ProcessAllModifiers(characterSO.GetPlayerStatData);
        CharacterManager.Init(CurrencyManager);


        AccountLevelManager.OnLevelUp += StatPoints.AddPoints;
     
        onComplete?.Invoke();
    }

    public void Reset()
    {
        StatPoints.OnValueChanged -= StatManager.ProcessStatPointsModifiers;
        CharacterManager.OnCharacterChanged -= (characterSO) => StatManager.ProcessAllModifiers(characterSO.GetPlayerStatData);
        AccountLevelManager.OnLevelUp -= StatPoints.AddPoints;
    }
}
