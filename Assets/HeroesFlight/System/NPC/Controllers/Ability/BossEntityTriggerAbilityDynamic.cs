using System;
using Cinemachine;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using StansAssets.Foundation.Async;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers.Ability
{
    public class BossEntityTriggerAbilityDynamic : BossAttackAbilityBase
    {
        [SerializeField] GameObject entityHolder;
        [SerializeField] float preDamageDelay = 2f;

        [Header("Parameters for optional visual zone usage")]
        [SerializeField] WarningLine zone;

        [SerializeField] float zoneWidth;

        AreaDamageEntity[] mushrooms;
        GameObject runtimeEntityHolder;
        protected override void Awake()
        {
            currentCooldown = 0;
            animator = GetComponentInParent<AiAnimatorInterface>();
            runtimeEntityHolder = Instantiate(entityHolder, transform.position, Quaternion.identity);
            mushrooms = runtimeEntityHolder.GetComponentsInChildren<AreaDamageEntity>();
            foreach (var mushroom in mushrooms)
            {
                mushroom.Init();
                mushroom.OnTargetsDetected += HandleTargetsDetected;
            }
          
        }

        void HandleTargetsDetected(int count, Collider2D[] targets)
        {
            for (int i = 0; i < count; i++)
            {
                if(targets[i].TryGetComponent<IHealthController>(out var health))
                {
                    health.TryDealDamage(new HealthModificationIntentModel(CalculateDamage(),
                        DamageType.NoneCritical,AttackType.Regular,DamageCalculationType.Flat));
                }
            }
        }


        public override void UseAbility(Action onComplete = null)
        {
            base.UseAbility(onComplete);
            runtimeEntityHolder.transform.position = transform.position;
            foreach (var mushroom in mushrooms)
            {
                mushroom.ToggleIndicator(true);
            }
            if (zone == null)
            {
                
                CoroutineUtility.WaitForSeconds(preDamageDelay,() =>
                {
                    cameraShaker?.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion,.5f);
                    foreach (var mushroom in mushrooms)
                    {
                        mushroom.StartDetection();
                    }
                });
            }
            else
            {
                zone.Trigger(() =>
                {
                    cameraShaker?.ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes.Explosion,.5f);
                    foreach (var mushroom in mushrooms)
                    {
                        mushroom.StartDetection();
                    }
                   
                },preDamageDelay,zoneWidth);
            }
           
            
        }
        
        
        public override void StopAbility()
        {
           
            foreach (var mushroom in mushrooms)
            {
                mushroom.gameObject.SetActive(false);
            }
        }
    }
}