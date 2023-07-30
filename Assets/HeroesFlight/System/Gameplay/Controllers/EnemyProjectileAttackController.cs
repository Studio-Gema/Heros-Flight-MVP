using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class EnemyProjectileAttackController : EnemyAttackControllerBase
    {
        [SerializeField] ProjectileControllerBase projectilePrefab;


        protected override void Update()
        {
            if (health.IsDead())
                return;

            if (target.IsDead())
            {
                aiController.SetAttackState(false);
                return;
            }


            timeSinceLastAttack += Time.deltaTime;
            var distanceToPlayer = Vector2.Distance(transform.position, aiController.CurrentTarget.position);
            if (distanceToPlayer <= attackRange && timeSinceLastAttack >= timeBetweenAttacks)
            {
                aiController.SetAttackState(true);
                InitAttack();
            }
        }


        protected override void InitAttack()
        {
            timeSinceLastAttack = 0;
            aiController.SetAttackState(false);
            animator.StartAttackAnimation(() =>
            {
                var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                projectile.SetupProjectile(Damage, aiController.CurrentTarget,
                    aiController.CurrentTarget.position - transform.position);
            });
        }
    }
}