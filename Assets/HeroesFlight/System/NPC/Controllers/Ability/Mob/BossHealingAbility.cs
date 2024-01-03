﻿using System;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Controllers.Control;
using HeroesFlightProject.System.Gameplay.Controllers;
using StansAssets.Foundation.Async;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers.Ability.Mob
{
    public class BossHealingAbility : AbilityBaseNPC
    {
        [SerializeField] private int maxUes = 1;
        [SerializeField] private float healAmount = 20f;
        [SerializeField] private float healthThresholdToUse = 50f;
        [SerializeField] private float preHealDelay = 1f;
        [SerializeField] private int currentUses;
        private BossController bossController;

        public override bool ReadyToUse =>
            bossController.CurrentHealthPercentage <= healthThresholdToUse && currentUses > 0;

        protected override void Awake()
        {
            base.Awake();
            currentUses = maxUes;
            bossController = GetComponentInParent<BossController>();
        }

        public override void UseAbility(Action onComplete = null)
        {
            Debug.Log("Trying to heal?");
            if (bossController.CurrentHealthPercentage > healthThresholdToUse)
            {
                onComplete?.Invoke();
                return;
            }

            base.UseAbility(onComplete);
            currentUses--;
            TryHealCrystals();
        }

        private void TryHealCrystals()
        {
            CoroutineUtility.WaitForSeconds(preHealDelay, () =>
            {
                foreach (var controller in bossController.CrystalNodes)
                {
                    if (controller.IsDead())
                        continue;

                    controller.TryDealDamage(new HealthModificationIntentModel(healAmount, DamageCritType.NoneCritical,
                        AttackType.Healing, CalculationType.Percentage, null, 1, .1f));
                }
                
                if (abilityParticle != null)
                {
                    abilityParticle.Play();
                }
                bossController.NotifyHealthChange();
            });
          
        }
    }
}