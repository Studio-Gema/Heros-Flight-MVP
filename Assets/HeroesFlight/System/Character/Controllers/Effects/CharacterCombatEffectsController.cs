﻿using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Effects.Effects;
using HeroesFlight.System.Combat.Effects.Effects.Data;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;

namespace HeroesFlight.System.Character.Controllers.Effects
{
    public class CharacterCombatEffectsController : CombatEffectsController
    {
        private CharacterControllerInterface controller;
        private CharacterStatController statController;

        protected override void Awake()
        {
            base.Awake();
            controller = GetComponent<CharacterSimpleController>();
            statController = GetComponent<CharacterStatController>();
        }

        protected override void HandleStatusEffectTick(StatusEffectRuntimeModel effectModel)
        {
            base.HandleStatusEffectTick(effectModel);

            switch (effectModel.Effect.EffectType)
            {
                case EffectType.Burn:
                    var data = effectModel.Effect.GetData<BurnEffectData>();

                    healthController.TryDealDamage(new HealthModificationIntentModel(
                        data.Damage.GetCurrentValue(effectModel.LVL) * effectModel.CurrentStacks,
                        DamageCritType.NoneCritical, AttackType.Regular, effectModel.Effect.CalculationType, null));
                    break;
                case EffectType.Root:
                    var rootData = effectModel.Effect.GetData<RootEffectData>();
                    healthController.TryDealDamage(new HealthModificationIntentModel(
                        rootData.Damage.GetCurrentValue(effectModel.LVL),
                        DamageCritType.NoneCritical, AttackType.DoT, effectModel.Effect.CalculationType, null));
                    break;
                case EffectType.Poison:
                    var poisonData = effectModel.Effect.GetData<PoisonEffectData>();
                    healthController.TryDealDamage(new HealthModificationIntentModel(
                        poisonData.Damage.GetCurrentValue(effectModel.LVL) * effectModel.CurrentStacks,
                        DamageCritType.NoneCritical, AttackType.DoT, effectModel.Effect.CalculationType, null));
                    break;
            }
        }


        protected override void HandleStatusEffectRemoval(StatusEffectRuntimeModel effectModel)
        {
            switch (effectModel.Effect.EffectType)
            {
                case EffectType.Root:
                    controller.SetActionState(true);
                    break;
            }
        }

        protected override void ApplyRootEffect(StatusEffect effect, out GameObject visual, int modelLvl)
        {
            base.ApplyRootEffect(effect, out visual, modelLvl);
            controller.SetActionState(false);
        }

        protected override void ApplyEffectOnInit(CombatEffect combatEffect, int lvl)
        {
            foreach (var effect in combatEffect.StatusEffects)
            {
                if (effect.GetType() == typeof(StatModificationEffect))
                {
                    var data = effect.GetData<StatModificationData>();
                    Debug.Log($"Modifiying {data.TargetStat}");
                    statController.GetStatModel.ModifyCurrentStat(data.TargetStat, data.StatValue.GetCurrentValue(lvl),
                        StatModel.StatModificationType.Addition, data.CalculationType);
                }
            }
        }

        protected override void RemoveCombatEffectsValues(CombatEffectRuntimeModel runtimeEffect)
        {
            foreach (var effect in runtimeEffect.Effect.StatusEffects)
            {
                if (effect.GetType() == typeof(StatModificationEffect))
                {
                    var data = effect.GetData<StatModificationData>();
                    statController.GetStatModel.ModifyCurrentStat(data.TargetStat,
                        data.StatValue.GetCurrentValue(runtimeEffect.Lvl),
                        StatModel.StatModificationType.Subtraction, data.CalculationType);
                }
            }
        }
    }
}