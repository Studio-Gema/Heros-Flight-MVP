using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RegularActiveAbility : MonoBehaviour
{
    [SerializeField] protected int currentLevel = 1;
    [SerializeField] GameObject[] effectParticle;
    protected int maxLevel = 9;
    protected int majorBoostLevel = 3;

    private RegularActiveAbilitySO activeAbilitySO;

    public RegularActiveAbilityType PassiveActiveAbilityType => activeAbilitySO.GetAbilityVisualData.RegularActiveAbilityType;
    public int Level => currentLevel;
    public RegularActiveAbilitySO ActiveAbilitySO => activeAbilitySO;

    public void Init(RegularActiveAbilitySO activeAbilitySO)
    {
        this.activeAbilitySO = activeAbilitySO;
    }

    public abstract void OnActivated();
    public abstract void OnDeactivated();
    public abstract void OnCoolDownEnded();

    public virtual void LevelUp()
    {
        if (currentLevel >= maxLevel)
        {
            return;
        }
        currentLevel++;
    }

    public int GetMajorValueByLevel(int baseValue, int increasePerLevel)
    {
        return baseValue + (Mathf.FloorToInt((currentLevel - 1) / majorBoostLevel) * increasePerLevel);
    }

    public float GetMajorValueByLevel(float baseValue, float increasePerLevel)
    {
        return baseValue + (Mathf.FloorToInt((currentLevel - 1) / majorBoostLevel) * increasePerLevel);
    }

    public GameObject GetEffectParticleByLevel()
    {
        return effectParticle[Mathf.FloorToInt((currentLevel - 1) / majorBoostLevel)];
    }
}
