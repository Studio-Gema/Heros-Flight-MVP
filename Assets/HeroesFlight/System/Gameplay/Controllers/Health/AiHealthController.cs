using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.NPC.Controllers;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AiHealthController : HealthController
    {
        AiControllerInterface aiController;

        public override void Init()
        {
            aiController = GetComponent<AiControllerInterface>();
            maxHealth = aiController.AgentModel.CombatModel.GetMonsterStatData.Health;
            heathBarUI?.ChangeType(HeathBarUI.HealthBarType.ToggleVisibilityOnHit);
            base.Init();
        }

        protected override void ProcessDeath()
        {
            base.ProcessDeath();
            aiController.Disable();
        }

        public override void DealDamage(DamageModel damage)
        {
            aiController.ProcessKnockBack();
            base.DealDamage(damage);
        }
    }
}