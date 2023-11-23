﻿using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlight.System.NPC.Controllers.Ability.Mob
{
    public class DamageDashAbility : AttackAbilityBaseNPC
    {
        [SerializeField] GameObject targetObject;
        [SerializeField] private float dashForce=100;
        [SerializeField] Trigger2DObserver observer;

        private Rigidbody2D rigidbody2D;
        private IHealthController healthController;
        private EnemyAttackControllerBase attackController;

        protected override void Awake()
        {
            animator = GetComponentInParent<AiAnimatorInterface>();
            attackController = GetComponentInParent<EnemyAttackControllerBase>();
            healthController = GetComponentInParent<AiHealthController>();
            rigidbody2D = GetComponentInParent<Rigidbody2D>();
            currentCooldown = 0;
            
        }
        
        public override void UseAbility(Action onComplete = null)
        {
            observer.OnEnter += HandleTargetEntered;
            targetObject.SetActive(true);
            if (targetAnimation != null)
            {
                animator.PlayDynamicAnimation(targetAnimation, () =>
                {
                    rigidbody2D.velocity=Vector2.zero;
                    observer.OnEnter -= HandleTargetEntered;
                    targetObject.SetActive(false);
                    onComplete?.Invoke();
                });
                var direction = (attackController.Target.HealthTransform.position - transform.position).normalized;
                rigidbody2D.AddForce(direction * dashForce, ForceMode2D.Impulse);
                currentCooldown = CoolDown;
            }
        }

        private void HandleTargetEntered(Collider2D obj)
        {
            Debug.Log("Trigered");
            var health = obj.GetComponent<IHealthController>();
            if (health != attackController.Target)
                return;
            
            
            if (canCrit)
            {
                bool isCritical = Random.Range(0, 100) <= critChance;

                float damageToDeal = isCritical
                    ? damage * critModifier
                    : damage;

                var type = isCritical ? DamageCritType.Critical : DamageCritType.NoneCritical;
                var damageModel = new HealthModificationIntentModel(damageToDeal,
                    type, AttackType.Regular, CalculationType.Flat, healthController);
                health.TryDealDamage(damageModel);
            }
            else
            {
                health.TryDealDamage(new HealthModificationIntentModel(damage,
                    DamageCritType.NoneCritical, AttackType.Regular, CalculationType.Flat, healthController));
            }
        }
    }
}