using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelEffectManager : MonoBehaviour
{
    public Action OnTrigger;
    public Action<AngelCard> OnPermanetCard;

    private List<AngelCard> collectedAngelCards = new List<AngelCard>();
    private List<AngelCard> permanetStatEffect = new List<AngelCard>();
    private CharacterStatController characterStatController;
    private MonsterStatController  monsterStatController;
    private AngelCard currentAngelCard;

    public bool EffectActive => currentAngelCard != null && currentAngelCard.angelCardSO != null;

    private void Awake()
    {
        monsterStatController = FindAnyObjectByType<MonsterStatController>();
    }

    public void Initialize(CharacterStatController characterStatController)
    {
        this.characterStatController = characterStatController;
    }

    public bool CompletedLevel()
    {
        if (currentAngelCard != null && currentAngelCard.angelCardSO != null)
        {
            foreach (StatEffect effect in currentAngelCard.angelCardSO.Effects)
            {
                RemoveEffect(currentAngelCard.tier, effect);
            }
            AddAfterBonusEffect(currentAngelCard.tier, currentAngelCard);
            return true;
        }

        return false;
    }

    private void AddAfterBonusEffect(AngelCardTier angelCardTier, AngelCard newAngelCard)
    {
        foreach (AngelCard angelCard in permanetStatEffect)
        {
            if (angelCard.angelCardSO == newAngelCard.angelCardSO)
            {
                ModifyPlayerStatRaw(angelCard.tier, angelCard.angelCardSO.AffterBonusEffect, false);
                permanetStatEffect.Remove(angelCard);
                break;
            }
        }

        permanetStatEffect.Add(newAngelCard);
        ModifyPlayerStatRaw(angelCardTier, newAngelCard.angelCardSO.AffterBonusEffect, true);
        OnPermanetCard?.Invoke(newAngelCard);
        currentAngelCard = null;
        characterStatController.SetCurrentCardIcon(null);
        monsterStatController.SetCurrentCardIcon(null);
    }

    public void AddAngelCardSO(AngelCardSO angelCardSO)
    {
        if (collectedAngelCards.Count == 0 || !CardExists(angelCardSO))
        {
            AngelCard angelCard = new AngelCard(angelCardSO);
            collectedAngelCards.Add(angelCard);

            foreach (StatEffect effect in angelCardSO.Effects)
            {
                AddEffect(angelCard.tier, effect);
            }

            currentAngelCard = angelCard;
            characterStatController.SetCurrentCardIcon(angelCardSO.CardImage);
            monsterStatController.SetCurrentCardIcon(angelCardSO.CardImage);
        }
    }

    private bool CardExists(AngelCardSO angelCardSO)
    {
        foreach (AngelCard angelCard in collectedAngelCards)
        {
            if (angelCard.angelCardSO == angelCardSO)
            {
                angelCard.tier++;
                foreach (StatEffect effect in angelCardSO.Effects)
                {
                    AddEffect(angelCard.tier, effect);
                }
                currentAngelCard = angelCard;
                return true;
            }
        }

        return false;
    }

    public AngelCard Exists(AngelCardSO angelCardSO)
    {
        foreach (AngelCard angelCard in collectedAngelCards)
        {
            if (angelCard.angelCardSO == angelCardSO)
            {
                return angelCard;
            }
        }

        return null;
    }

    private void AddEffect(AngelCardTier angelCardTier, StatEffect effect)
    {
        switch (effect.targetType)
        {
            case TargetType.All:
                ModifyPlayerStatDifference(angelCardTier, effect, true);
                ModifyMonsterStatRaw(angelCardTier, effect, true);
                break;
            case TargetType.Player:
                ModifyPlayerStatDifference(angelCardTier, effect,true);
                break;
            case TargetType.Monster:
                ModifyMonsterStatRaw(angelCardTier, effect, true);
                break;
        }
    }

    private void RemoveEffect(AngelCardTier angelCardTier, StatEffect effect)
    {
        switch (effect.targetType)
        {
            case TargetType.All:
                ModifyPlayerStatRaw(angelCardTier, effect,false);
                ModifyMonsterStatRaw(angelCardTier, effect,false);
                break;
            case TargetType.Player:
                ModifyPlayerStatRaw(angelCardTier, effect, false);
                break;
            case TargetType.Monster:
                ModifyMonsterStatRaw(angelCardTier, effect, false);
                break;
        }
    }

    private void ModifyPlayerStatDifference(AngelCardTier angelCardTier, StatEffect effect, bool positive)
    {
        switch (effect.effect)
        {
            case BuffDebuff.AttackUp:
                characterStatController.ModifyPhysicalDamage(effect.GetValueDifference(angelCardTier), positive);
                characterStatController.ModifyMagicDamage(effect.GetValueDifference(angelCardTier), positive);
                break;
            case BuffDebuff.AttackDown:
                characterStatController.ModifyPhysicalDamage(effect.GetValueDifference(angelCardTier), !positive);
                characterStatController.ModifyMagicDamage(effect.GetValueDifference(angelCardTier), !positive);
                break;
            case BuffDebuff.DefenseUp:
                characterStatController.ModifyDefense(effect.GetValueDifference(angelCardTier), positive);
                break;
            case BuffDebuff.DefenseDown:
                characterStatController.ModifyDefense(effect.GetValueDifference(angelCardTier), !positive);
                break;
            case BuffDebuff.AttackSpeedUp:
                characterStatController.ModifyAttackSpeed(effect.GetValueDifference(angelCardTier), positive);
                break;
            case BuffDebuff.AttackSpeedDown:
                characterStatController.ModifyAttackSpeed(effect.GetValueDifference(angelCardTier), !positive);
                break;
        }
    }

    private void ModifyPlayerStatRaw(AngelCardTier angelCardTier, StatEffect effect, bool positive)
    {
        switch (effect.effect)
        {
            case BuffDebuff.AttackUp:
                characterStatController.ModifyPhysicalDamage(effect.GetValue(angelCardTier), positive);
                characterStatController.ModifyMagicDamage(effect.GetValue(angelCardTier), positive);
                break;
            case BuffDebuff.AttackDown:
                characterStatController.ModifyPhysicalDamage(effect.GetValue(angelCardTier), !positive);
                characterStatController.ModifyMagicDamage(effect.GetValue(angelCardTier), !positive);
                break;
            case BuffDebuff.DefenseUp:
                characterStatController.ModifyDefense(effect.GetValue(angelCardTier), positive);
                break;
            case BuffDebuff.DefenseDown:
                characterStatController.ModifyDefense(effect.GetValue(angelCardTier), !positive);
                break;
            case BuffDebuff.AttackSpeedUp:
                characterStatController.ModifyAttackSpeed(effect.GetValue(angelCardTier), positive);
                break;
            case BuffDebuff.AttackSpeedDown:
                characterStatController.ModifyAttackSpeed(effect.GetValue(angelCardTier), !positive);
                break;
        }
    }

    private void ModifyMonsterStatRaw(AngelCardTier angelCardTier, StatEffect effect, bool positive)
    {
        switch (effect.effect)
        {
            case BuffDebuff.AttackUp:
                monsterStatController.ModifyAttackModifier(effect.GetValue(angelCardTier), positive);
                break;
            case BuffDebuff.AttackDown:
                monsterStatController.ModifyAttackModifier(effect.GetValue(angelCardTier), !positive);
                break;
            case BuffDebuff.DefenseUp:
                monsterStatController.ModifyDefenseModifier(effect.GetValue(angelCardTier), positive);
                break;
            case BuffDebuff.DefenseDown:    
                monsterStatController.ModifyDefenseModifier(effect.GetValue(angelCardTier), !positive);
                break;
            case BuffDebuff.AttackSpeedUp:
                monsterStatController.ModifyAttackSpeedModifier(effect.GetValue(angelCardTier), positive);
                break;
            case BuffDebuff.AttackSpeedDown:
                monsterStatController.ModifyAttackSpeedModifier(effect.GetValue(angelCardTier), !positive);
                break;
        }
    }

    public void TriggerAngelsGambit()
    {
        OnTrigger?.Invoke();
    }

    public AngelCard GetActiveAngelCard()
    {
        return currentAngelCard;
    }

    public void ResetAngelEffects()
    {
        //foreach (AngelCard angelCard in collectedAngelCards)
        //{
        //    foreach (StatEffect effect in angelCard.angelCardSO.Effects)
        //    {
        //        RemoveEffect(angelCard.tier, effect);
        //    }
        //}
        collectedAngelCards = new List<AngelCard>();
        permanetStatEffect = new List<AngelCard>();
        currentAngelCard = null;
        Debug.Log("Reset Angel Effects");
    }
}