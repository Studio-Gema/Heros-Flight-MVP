using HeroesFlight.System.FileManager;
using System;
using UnityEngine;

public class AccountLevelManager : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        public int currentLevel;
        public float currentExp;
    }

    public event Action<LevelSystem.ExpIncreaseResponse> OnLevelUp;

    public const string SAVE_ID = "AccountLevelData";

    [Header("Curves")]
    [SerializeField] private CustomAnimationCurve levelCurve;
    [SerializeField] private CustomAnimationCurve gemCurve;
    [SerializeField] private CustomAnimationCurve goldCurve;

    private LevelSystem levelSystem;

    [Header("Debug")]
    [SerializeField] private Data data;

    public int CurrentPlayerLvl => levelSystem.CurrentLevel;
    
    public void Init()
    {
        Load();

        levelSystem.OnLevelUp += (response) =>
        {
            OnLevelUp?.Invoke(response);
        };
    }

    //FOR DEBUG
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddExp(1500);
        }
    }

    public int GetGemReward()
    {
        return gemCurve.GetCurrentValueInt(levelSystem.CurrentLevel);
    }

    public int GetGoldReward()
    {
        return goldCurve.GetCurrentValueInt(levelSystem.CurrentLevel);
    }

    public LevelSystem.ExpIncreaseResponse GetExpIncreaseResponse()
    {
        return new LevelSystem.ExpIncreaseResponse
        {
            numberOfLevelsGained = 0,
            currentLevel = levelSystem.CurrentLevel,
            normalizedExp = levelSystem.GetNormalizedExp()
        };
    }

    public void AddExp(float exp)
    {
        levelSystem.AddExp(exp);
        Save();
    }

    public void SetXp(float exp)
    {
        levelSystem.SetExp(exp);
        Save();
    }

    public void Load()
    {
        Data savedData = FileManager.Load<Data>(SAVE_ID);
        data = savedData != null ? savedData : new Data();
        levelSystem = new LevelSystem(data.currentLevel, data.currentExp, levelCurve);
    }

    public void Save()
    {
        data.currentLevel = levelSystem.CurrentLevel;
        data.currentExp = levelSystem.CurrentExp;
        FileManager.Save(SAVE_ID, data);
    }

    private void OnDrawGizmosSelected()
    {
        if (levelCurve != null)
        {
            levelCurve.UpdateCurve();
        }

        if (gemCurve != null)
        {
            gemCurve.UpdateCurve();
        }

        if (goldCurve != null)
        {
            goldCurve.UpdateCurve();
        }
    }
}