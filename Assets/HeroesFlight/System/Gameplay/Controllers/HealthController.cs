using System;
using HeroesFlight.Common.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class HealthController : MonoBehaviour, IHealthController
    {
        [SerializeField] CombatTargetType targetType;
        public CombatTargetType TargetType => targetType;
        public int CurrentHealth => currentHealh;
        protected int maxHealth;
         [SerializeField]  protected int currentHealh;
        public event Action<int> OnBeingDamaged;
        public event Action OnDeath;

        void Awake()
        {
            Init();
        }

        public virtual void Init()
        {
            currentHealh = maxHealth;
        }

        public virtual void DealDamage(int damage)
        {
            Debug.Log($"received {damage}");
            OnBeingDamaged?.Invoke(damage);
            currentHealh -= damage;
            if (IsDead())
                ProcessDeath();
        }

        public virtual bool IsDead()
        {
            return currentHealh <= 0;
        }

        protected virtual void ProcessDeath()
        {
            OnDeath?.Invoke();
        }
    }
}