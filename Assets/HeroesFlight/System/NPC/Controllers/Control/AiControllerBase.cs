using System;
using HeroesFlight.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Data;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public abstract class AiControllerBase : MonoBehaviour, AiControllerInterface
    {
        [SerializeField] protected SpriteRenderer buffDebuffIcon;
        [SerializeField] protected AiAgentModel m_Model;
        protected FlashEffect hitEffect;
        protected AiViewController viewController;
        protected AiAnimatorInterface animator;
        protected Collider2D attackCollider;
        protected bool isInknockback;
        public event Action OnInitialized;
        public event Action<AiControllerInterface> OnDisabled;
        public EnemyType EnemyType => m_Model.EnemyType;
        public AiAgentModel AgentModel => m_Model;
        public Transform CurrentTarget => currentTarget;

        public int GetHealth => currentHealth;

        public float GetDamage => currentDamage;

        protected Rigidbody2D rigidBody;
        protected Transform currentTarget;
        protected bool isDisabled;
        protected bool isAggravated;
        protected MonsterStatModifier statModifier;
        bool canAttack;
        Vector2 wanderPosition;
        float timeSinceAggravated = Mathf.Infinity;

        protected int currentHealth;
        protected float currentDamage;

        public virtual void Init(Transform player,int health, float damage, MonsterStatModifier monsterStatModifier, Sprite currentCardIcon)
        {
            statModifier = EnemyType == EnemyType.MiniBoss ? new MonsterStatModifier() : monsterStatModifier;
            rigidBody = GetComponent<Rigidbody2D>();
            attackCollider = GetComponent<Collider2D>();
            animator = GetComponent<AiAnimatorInterface>();
            viewController = GetComponent<AiViewController>();
            hitEffect = GetComponentInChildren<FlashEffect>();
            currentTarget = player;
            viewController.Init();

            currentHealth = Mathf.RoundToInt(statModifier.CalculateAttack(health));
            
            currentDamage = statModifier.CalculateAttack(damage);

            OnInit();

            // viewController.StartFadeIn(2f, Enable);
            DisplayModifiyer(currentCardIcon);
            Enable();
        }

        void Update()
        {
            if (isDisabled)
                return;

            if (!IsAggravated() || !canAttack)
            {
                ProcessWanderingState();
            }
            else
            {
                ProcessFollowingState();
            }

            UpdateTimers();
        }

        void UpdateTimers()
        {
            timeSinceAggravated += Time.deltaTime;
        }


        public virtual Vector2 GetVelocity()
        {
            return Vector2.zero;
        }

        public void SetAttackState(bool attackState)
        {
            canAttack = attackState;
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


        public virtual void ProcessWanderingState()
        {
        }

        public virtual void ProcessFollowingState()
        {
        }

        protected bool IsAggravated()
        {
            var distance = Vector2.Distance(CurrentTarget.position, transform.position);
            return distance <= m_Model.AgroDistance ||
                timeSinceAggravated < m_Model.AgroDuration;
        }

        protected bool InAttackRange()
        {
            return Vector2.Distance(CurrentTarget.position, transform.position)
                <= m_Model.AiData.AttackRange;
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
        }
    }
}