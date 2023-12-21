﻿using System;
using System.Collections.Generic;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers.Control
{
    public class BossControllerBase : MonoBehaviour,BossControllerInterface
    {
        public event Action<Transform> OnCrystalDestroyed;
        public BossState State { get; protected set; }
        public List<IHealthController> CrystalNodes { get;protected set; }
        public event Action<float> OnHealthPercentageChange;
        public event Action<HealthModificationIntentModel> OnBeingDamaged;
        public event Action<BossState> OnBossStateChange;
        public virtual void Init() { }

        protected void InvokeHealthPercChangeEvent(float value)
        {
            OnHealthPercentageChange?.Invoke(value);
        }

        protected void InvokeOnBeingDamagedEvent(HealthModificationIntentModel model)
        {
            OnBeingDamaged?.Invoke(model);
        }

        protected void InvokeCrystalDestroyedEvent(Transform target)
        {
            OnCrystalDestroyed?.Invoke(target);
        }
        
        protected void ChangeState(BossState newState)
        {
            if (State == newState)
                return;

            State = newState;
            Debug.Log(newState);
            OnBossStateChange?.Invoke(State);
        }
    }
}