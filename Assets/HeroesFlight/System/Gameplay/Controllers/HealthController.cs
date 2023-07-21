using System;
using HeroesFlight.Common.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class HealthController : MonoBehaviour,IHealthController
    {
        [SerializeField] CombatTargetType targetType;
        public CombatTargetType TargetType => targetType;
        public int CurrentHealth => currentHealh;
        int maxHealth;
        int currentHealh;
        public void Init(int maxHealth)
        {
            throw new NotImplementedException();
        }

        public event Action OnBeingDamaged;
        public event Action OnDeath;

        public void DealDamage(int damage)
        {
            Debug.Log($"received {damage}");
        }

        public bool IsDead()
        {
            throw new NotImplementedException();
        }
    }

}
