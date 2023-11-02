using System;
using HeroesFlight.System.NPC.Controllers;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Data;
using HeroesFlightProject.System.NPC.Enum;
using HeroesFlightProject.System.NPC.State;
using UnityEditor;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public abstract class AiControllerBase : MonoBehaviour, AiControllerInterface
    {
        [SerializeField] protected SpriteRenderer buffDebuffIcon;

        [SerializeField] protected AiAgentModel m_Model;

      
        public event Action OnInitialized;
        public event Action<AiControllerInterface> OnDisabled;
        public EnemyType EnemyType => m_Model.EnemyType;
        public AiAgentModel AgentModel => m_Model;
        public Transform CurrentTarget => currentTarget;

        public int GetHealth => currentHealth;

        public float GetDamage => currentDamage;

        protected FlashEffect hitEffect;
        protected AiViewController viewController;
        protected AiAnimatorInterface animator;
        protected Collider2D attackCollider;
        protected bool isInknockback;
        protected Rigidbody2D rigidBody;
        protected Transform currentTarget;
        protected MonsterStatModifier statModifier;
        protected int currentHealth;
        protected float currentDamage;
        protected bool isDisabled;
        protected FSMachine stateMachine;
        protected AiMoverInterface mover;
        protected EnemyAttackControllerBase attacker;

        protected AiHealthController healthController;

        float timeSinceAggravated = Mathf.Infinity;
        Vector2 wanderPosition;
      
        public virtual void Init(Transform player, int health, float damage, MonsterStatModifier monsterStatModifier,
            Sprite currentCardIcon)
        {
            statModifier = EnemyType == EnemyType.MiniBoss ? new MonsterStatModifier() : monsterStatModifier;
            rigidBody = GetComponent<Rigidbody2D>();
            attackCollider = GetComponent<Collider2D>();
            animator = GetComponent<AiAnimatorInterface>();
            viewController = GetComponent<AiViewController>();
            hitEffect = GetComponentInChildren<FlashEffect>();
            healthController = GetComponent<AiHealthController>();
            healthController.SetHealthStats(health,
                GetMonsterStatModifier().CalculateDefence(AgentModel.AiData.Defense));
            healthController.OnDeath += HandleDeath;
            mover = GetComponent<AiMoverInterface>();
            mover.Init(m_Model);
            attacker = GetComponent<EnemyAttackControllerBase>();
            currentTarget = player;
            viewController.Init();

            currentHealth = Mathf.RoundToInt(statModifier.CalculateAttack(health));

            currentDamage = statModifier.CalculateAttack(damage);
            attacker.SetAttackStats(statModifier.CalculateAttack(damage), m_Model.AiData.AttackRange,
                m_Model.AiData.AttackSpeed, m_Model.AiData.CriticalHitChance);
            attacker.SetTarget(player.GetComponent<IHealthController>());
            OnInit();

            // viewController.StartFadeIn(2f, Enable);
            DisplayModifiyer(currentCardIcon);
            Enable();
        }

        protected virtual void HandleDeath(IHealthController obj)
        {
            Disable();
        }

        protected virtual void Update()
        {
            stateMachine?.Process();
            UpdateTimers();
            animator.SetMovementDirection(GetVelocity());
        }

        void UpdateTimers()
        {
            timeSinceAggravated += Time.deltaTime;
        }


        protected virtual Vector2 GetVelocity()
        {
            return Vector2.zero;
        }


        public virtual void ProcessKnockBack()
        {
            animator.PlayHitAnimation(m_Model.AttacksInteruptable);
            hitEffect.Flash();
        }

        public virtual void Enable()
        {
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
            gameObject.SetActive(true);
            attackCollider.enabled = true;
            isDisabled = false;
        }

        public virtual void Disable()
        {
            isDisabled = true;
            attackCollider.enabled = false;
            //  rigidBody.bodyType = RigidbodyType2D.Static;
            animator.PlayDeathAnimation(() =>
            {
                if (gameObject != null)
                {
                    gameObject.SetActive(false);
                }

                OnDisabled?.Invoke(this);
            });
        }

        public bool TryGetController<T>(out T controller)
        {
            controller = default;
            if (TryGetComponent<T>(out controller))
            {
                return true;
            }

            return false;
        }


        

        public bool IsAggravated()
        {
            var distance = Vector2.Distance(CurrentTarget.position, transform.position);
            return distance <= m_Model.AgroDistance ||
                   timeSinceAggravated < m_Model.AgroDuration;
        }


        public void DisplayModifiyer(Sprite sprite)
        {
            // TODO: Fix this
            buffDebuffIcon.enabled = false;

            //if (sprite == null)
            //{
            //    buffDebuffIcon.enabled = false;
            //}
            //else
            //{
            //    buffDebuffIcon.enabled = true;
            //    buffDebuffIcon.sprite = sprite;
            //}
        }

        protected void OnInit()
        {
            OnInitialized?.Invoke();
        }

        public MonsterStatModifier GetMonsterStatModifier()
        {
            return statModifier;
        }

        public void Aggravate()
        {
            timeSinceAggravated = 0;
        }

        public virtual void SetMovementState(bool canMove)
        {
            animator.SetMovementAnimation(canMove);
            mover.SetMovementState(canMove);
        }

        private void OnDrawGizmosSelected()
        {
            if (stateMachine != null && stateMachine.CurrentState != null)
            {
                Handles.Label(transform.position, stateMachine.CurrentState.GetType().ToString());
            }
        }
    }
}