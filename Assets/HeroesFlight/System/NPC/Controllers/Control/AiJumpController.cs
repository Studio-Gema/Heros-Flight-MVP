using HeroesFlight.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public class AiJumpController : AiControllerBase
    {
        
        public override void Init(Transform player, int health, float damage, MonsterStatModifier monsterStatModifier, Sprite currentCardIcon)
        {
            statModifier = monsterStatModifier;
            mover = GetComponent<AiMoverInterface>();
            rigidBody = GetComponent<Rigidbody2D>();
            attackCollider = GetComponent<Collider2D>();
            animator = GetComponent<AiAnimatorInterface>();
            rigidBody.isKinematic = true;
            viewController = GetComponent<AiViewController>();
            viewController.Init();
            currentTarget = player;
            OnInit();
            viewController.StartFadeIn(2f,Enable);
            currentHealth = Mathf.RoundToInt(statModifier.CalculateAttack(health));
            currentDamage = statModifier.CalculateAttack(damage);
            DisplayModifiyer(currentCardIcon);
            Enable();
        }


        public  void ProcessFollowingState()
        {
            var directionToTarget = (Vector2)(CurrentTarget.position - transform.position);
            mover.MoveToTargetPosition(directionToTarget);
        }

        public  void ProcessWanderingState()
        {
            var random = Random.Range(0, 2);
            var directionToMove = random == 0 ? Vector2.left : Vector2.right;
            mover.MoveToTargetPosition(directionToMove);
        }

        public override void Enable()
        {
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
            gameObject.SetActive(true);
        }

        public override void Disable()
        {
            rigidBody.bodyType = RigidbodyType2D.Static;
            base.Disable();
        }

        public override void ProcessKnockBack()
        {
            base.ProcessKnockBack();
            rigidBody.velocity = Vector2.zero;
            var forceVector = currentTarget.position.x > transform.position.x ? Vector2.left : Vector2.right;
            forceVector.Normalize();
            rigidBody.AddForce(forceVector *m_Model.KnockBackForce,ForceMode2D.Impulse);
        }

        protected override Vector2 GetVelocity()
        {
            return mover.GetVelocity();
        }
    }
}