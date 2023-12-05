using Pelumi.ObjectPool;
using ScriptableObjectDatabase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Regular Active Ability", menuName = "Ability / Active Ability")]
public class ActiveAbilitySO : ScriptableObject, IHasID
{
    [SerializeField] ActiveAbilityVisualData abilityVisualData;
    [SerializeField] protected float duration;
    [SerializeField] protected int cooldown;
    [SerializeField] RegularActiveAbility passiveActiveAbility;

    public ActiveAbilityVisualData GetAbilityVisualData => abilityVisualData;
    public float Duration => duration;
    public int Cooldown => cooldown;

    public RegularActiveAbility GetAbility(Vector3 position, Quaternion rotation = default)
    {
        RegularActiveAbility ability = Instantiate(passiveActiveAbility, position, rotation);
        ability.Init(this);
        return ability;
    }

    public string GetID()
    {
        return abilityVisualData.ActiveAbilityType.ToString();
    }
}

public enum AbilityDurationType
{
    OverTime,
    Instant,
    Combined
}