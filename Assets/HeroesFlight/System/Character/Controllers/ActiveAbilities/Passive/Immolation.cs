using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Immolation : PassiveActiveAbility
{
    [SerializeField] private float damageRate;
    [SerializeField] private float damagePercentage;
    [SerializeField] private float damagePercentagePerIncrease;
    [SerializeField] private OverlapChecker overlapChecker;

    private int baseDamage;
    private float currentDamagePercentage;
    private int currentDamage;
    bool isOn = false;
    private float currentTime;

    public override void OnActivated()
    {
        GetEffectParticleByLevel().SetActive(true);
        currentDamagePercentage = GetMajorValueByLevel(damagePercentage, damagePercentagePerIncrease);
        currentDamage = (int)StatCalc.GetPercentage (baseDamage, currentDamagePercentage);
        isOn = true;
    }

    public override void OnCoolDownStarted()
    {
        GetEffectParticleByLevel().SetActive(false);
        isOn = false;
    }

    public override void OnCoolDownEnded()
    {

    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    public override void LevelUpIncreaseEffect()
    {

    }

    public void Initialize(int level, int baseDamage)
    {
        this.currentLevel = level;
        this.baseDamage = baseDamage;
        overlapChecker.OnDetect = OnDetect;
    }

    private void Update()
    {
        if (isOn)
        {
            if (currentTime <= 0)
            {
                currentTime = damageRate;
                overlapChecker.DetectOverlap();
            }
            else
            {
                currentTime -= Time.deltaTime;
            }
        }
    }

    private void OnDetect(int arg1, Collider2D[] collider2D)
    {
        for (int i = 0; i < arg1; i++)
        {
            if (collider2D[i].TryGetComponent(out IHealthController healthController))
            {
                
                healthController.TryDealDamage(new HealthModificationIntentModel(currentDamage,
                DamageType.NoneCritical, AttackType.Regular, DamageCalculationType.Flat));
            }
        }
    }
}