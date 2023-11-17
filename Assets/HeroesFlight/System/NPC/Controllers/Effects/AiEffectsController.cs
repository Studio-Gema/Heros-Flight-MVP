﻿using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Effects.Effects;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Controllers.Movement;
using HeroesFlightProject.System.NPC.Controllers;

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
                    healthController.TryDealDamage(new HealthModificationIntentModel(effectModel.Effect.Value *effectModel.CurrentStacks ,
                        DamageCritType.NoneCritical,AttackType.DoT,CalculationType.Percentage,null));
                    break;
                case EffectType.Freeze:
                    var speedModifier = controller.AgentModel.AiData.MoveSpeed/100 * effectModel.Effect.Value;
                    mover.SetMovementSpeed( controller.AgentModel.AiData.MoveSpeed-speedModifier);
                    break;
                case EffectType.Root:
                    break;
                case EffectType.Poison:
                    break;
                case EffectType.Shock:
                    break;
                case EffectType.Reflect:
                    healthController.TryDealDamage(new HealthModificationIntentModel(effectModel.Effect.Value,
                        DamageCritType.NoneCritical,AttackType.DoT,CalculationType.Flat,null));
                    break;
                case EffectType.Sacrifice:
                    break;
                case EffectType.FullCounter:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void HandleStatusEffectRemoval(StatusEffectRuntimeModel effectModel)
        {
            switch (effectModel.Effect.EffectType)
            {
                case EffectType.Freeze:
                    mover.SetMovementSpeed( controller.AgentModel.AiData.MoveSpeed);
                    break;
                case EffectType.Root:
                    break;
               
            }
        }
    }
}