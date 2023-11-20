﻿using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Effects.Effects;
using HeroesFlight.System.Combat.Effects.Effects.Data;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Controllers.Movement;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers.Effects
{
    public class AiEffectsController : CombatEffectsController
    {
        private AiControllerBase controller;
        private AiBaseMovementController mover;

        protected override void Awake()
        {
            base.Awake();
            controller = GetComponent<AiControllerBase>();
            mover = GetComponent<AiBaseMovementController>();
        }

        protected override void HandleStatusEffectTick(StatusEffectRuntimeModel effectModel)
        {
            switch (effectModel.Effect.EffectType)
            {
                case EffectType.Burn:
                    var burnData = effectModel.Effect.GetData<BurnEffectData>();
                    healthController.TryDealDamage(new HealthModificationIntentModel(
                        burnData.Damage.GetCurrentValue(1) * effectModel.CurrentStacks,
                        DamageCritType.NoneCritical, AttackType.DoT, effectModel.Effect.CalculationType, null));
                    break;

                case EffectType.Root:
                    healthController.TryDealDamage(new HealthModificationIntentModel(effectModel.Effect.Value,
                        DamageCritType.NoneCritical, AttackType.DoT, effectModel.Effect.CalculationType, null));
                    break;
                case EffectType.Poison:
                    var poisonData =effectModel.Effect.GetData<PoisonEffectData>();
                    healthController.TryDealDamage(new HealthModificationIntentModel(
                        poisonData.Damage.GetCurrentValue(1) * effectModel.CurrentStacks,
                        DamageCritType.NoneCritical, AttackType.DoT, effectModel.Effect.CalculationType, null));
                    break;
                case EffectType.Shock:
                    var shockData = effectModel.Effect.GetData<ShockEffectData>();
                    healthController.TryDealDamage(new HealthModificationIntentModel(
                        shockData.MainDamage.GetCurrentValue(1),
                        DamageCritType.NoneCritical, AttackType.DoT, effectModel.Effect.CalculationType, null));
                    var shockController = effectModel.Visual.GetComponent<ShockEffectController>();
                    ParticleManager.instance.Spawn(shockController.MainParticle,
                        healthController.HealthTransform.position);
                    shockController.TriggerEffect(healthController, new HealthModificationIntentModel(
                        shockData.SecondaryDamage.GetCurrentValue(1),
                        DamageCritType.NoneCritical, AttackType.DoT, effectModel.Effect.CalculationType, null));
                    break;
                case EffectType.Reflect:
                    healthController.TryDealDamage(new HealthModificationIntentModel(effectModel.Effect.Value,
                        DamageCritType.NoneCritical, AttackType.DoT, effectModel.Effect.CalculationType, null));
                    break;
            }
        }

        protected override void HandleStatusEffectRemoval(StatusEffectRuntimeModel effectModel)
        {
            switch (effectModel.Effect.EffectType)
            {
                case EffectType.Freeze:
                    if (mover == null)
                        return;
                    mover.SetMovementSpeed(controller.AgentModel.AiData.MoveSpeed);
                    break;
                case EffectType.Root:
                    mover.SetMovementState(true);
                    break;
            }
        }

        protected override void ApplyRootEffect(StatusEffect effect, out GameObject visual)
        {
            base.ApplyRootEffect(effect, out visual);
            mover.SetMovementState(false);
        }

        protected override void ApplyFreezeEffect(StatusEffect effect, out GameObject visual)
        {
            base.ApplyFreezeEffect(effect, out visual);
            if (mover == null)
                return;
            var data = effect.GetData<FreezeEffectData>();
            var speedModifier = controller.AgentModel.AiData.MoveSpeed / 100 * data.SlowAmount.GetCurrentValue(1);
            mover.SetMovementSpeed(controller.AgentModel.AiData.MoveSpeed - speedModifier);
        }
    }
}