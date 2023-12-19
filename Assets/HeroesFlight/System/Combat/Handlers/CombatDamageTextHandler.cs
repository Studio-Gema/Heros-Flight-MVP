﻿using System.Collections;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Container;
using HeroesFlight.System.Combat.Model;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.UI;
using UnityEngine;

namespace HeroesFlight.System.Combat.Handlers
{
    public class CombatDamageTextHandler
    {
        public CombatDamageTextHandler(IUISystem uiSystemInterface)
        {
            uiSystem = uiSystemInterface;
        }

        private IUISystem uiSystem;
        private CombatContainer combatContainer;

        public void ShowDamagedText(HealthModificationRequestModel model, bool isPlayer)
        {
            if (model.IntentModel.Amount == 0)
                return;
            combatContainer.StartCoroutine(DamageTextRoutine(model, isPlayer));
        }

        public void InjectContainer(CombatContainer container) => combatContainer = container;


        IEnumerator DamageTextRoutine(HealthModificationRequestModel model, bool isPlayer)
        {
            for (int i = 0; i < model.IntentModel.DamageInstancesCount; i++)
            {
                uiSystem.ShowDamageText(model.IntentModel.Amount, model.RequestOwner.HealthTransform,
                    model.IntentModel.DamageCritType == DamageCritType.Critical, isPlayer,
                    model.IntentModel.AttackType == AttackType.Healing);
                yield return new WaitForSeconds(model.IntentModel.DelayBetweenInstances);
            }
        }
    }
}