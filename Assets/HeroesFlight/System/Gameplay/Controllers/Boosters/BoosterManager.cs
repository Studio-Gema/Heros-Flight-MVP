using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterManager : MonoBehaviour
{
    [SerializeField] private CharacterStatController characterStatController;
    [SerializeField] private List<BoosterContainer> boosterContainerList;

    [SerializeField] private bool pauseAttackBooster;

    public bool PauseAttackBooster => pauseAttackBooster;

    public void Initialise(CharacterStatController statController)
    {
        characterStatController = statController;
    }

    public void ActivateBooster(BoosterSO boosterSO)
    {
        Boost boost = CreateBoost(boosterSO);

        if (IsInstantBoost(boosterSO))
        {
            boost.OnStart?.Invoke();
            return;
        }

        if(IsBoosterIsActive(boosterSO))
        {
            return;
        }

        BoosterContainer boosterContainer = boosterContainerList.Find(x => !x.IsRunning);

        if (boosterContainer == null)
        {
            boosterContainer = new BoosterContainer();
            boosterContainerList.Add(boosterContainer);
        }

        boosterContainer.SetActiveBoost(this, boost);
    }

    public bool IsInstantBoost(BoosterSO boosterSO)
    {
        return boosterSO.BoosterDuration == 0;
    }

    public bool IsBoosterIsActive(BoosterSO boosterSO)
    {
        foreach (var boosterContainer in boosterContainerList)
        {
            if (boosterContainer.IsRunning && boosterContainer.ActiveBoost.boosterSO == boosterSO)
            {
                switch (boosterContainer.ActiveBoost.boosterSO.BoosterStackType)
                {
                    case BoosterStackType.None:
                        return true;
                    case BoosterStackType.Duration:
                        boosterContainer.ResetBoostDuration();
                        return true;
                    case BoosterStackType.Effect:
                        boosterContainer.IncreaseStackCount();
                        return true;
                }
            }
        }
        return false;
    }

    public Boost CreateBoost(BoosterSO boosterSO)
    {
        switch (boosterSO.BoosterEffectType)
        {
            case BoosterEffectType.Health:
                return HealthBoost(boosterSO);
            case BoosterEffectType.Attack:
                return AttackBoost(boosterSO);
            case BoosterEffectType.Defense:
                return DefenseBoost(boosterSO);
            case BoosterEffectType.MoveSpeed:
                return MoveSpeedBoost(boosterSO);
            case BoosterEffectType.AttackSpeed:
                return AttackSpeedBoost(boosterSO);
            default: return null;
        }
    }

    public Boost DefenseBoost(BoosterSO boosterSO)
    {
        return new Boost(boosterSO, ()=>
        {
            characterStatController.ModifyDefense(boosterSO.BoosterValue, true);
        }, () =>
        {
            characterStatController.ModifyDefense(boosterSO.BoosterValue, false);
        });
    }

    public Boost AttackSpeedBoost(BoosterSO boosterSO)
    {
        return new Boost(boosterSO, () =>
        {
            characterStatController.ModifyAttackSpeed(boosterSO.BoosterValue, true);
        }, () =>
        {
            characterStatController.ModifyAttackSpeed(boosterSO.BoosterValue, false);
        });
    }

    public Boost MoveSpeedBoost(BoosterSO boosterSO)
    {
        return new Boost(boosterSO, () =>
        {
            characterStatController.ModifyMoveSpeed(boosterSO.BoosterValue, true);
        }, () =>
        {
            characterStatController.ModifyMoveSpeed(boosterSO.BoosterValue, false);
        });
    }

    public Boost HealthBoost (BoosterSO boosterSO)
    {
        return new Boost(boosterSO, () =>
        {
            if(characterStatController.GetHealthPercentage() >= 40)
            {
                characterStatController.ModifyHealth(boosterSO.BoosterValue, true);
            }
            else
            {
                characterStatController.ModifyHealth(boosterSO.BoosterValue  + 10, true);
            }

        }, null);
    }

    public Boost AttackBoost(BoosterSO boosterSO)
    {
        return new Boost(boosterSO, () =>
        {
            characterStatController.ModifyMagicDamage(boosterSO.BoosterValue, true);
            characterStatController.ModifyPhysicalDamage(boosterSO.BoosterValue, true);
            pauseAttackBooster = true;
        }, () =>
        {
            characterStatController.ModifyMagicDamage(boosterSO.BoosterValue, false);
            characterStatController.ModifyPhysicalDamage(boosterSO.BoosterValue, false);
            pauseAttackBooster = false;
        });
    }
}
