using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Controllers;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AiHealthController : HealthController, AiSubControllerInterface
    {
        AiControllerInterface aiController;

        public override void Init()
        {
            aiController = GetComponent<AiControllerInterface>();
            heathBarUI?.ChangeType(WorldBarUI.BarType.ToggleVisibilityOnHit);
            base.Init();
        }


        public override void ModifyHealth(HealthModificationIntentModel modificationIntentModel)
        {
          
            base.ModifyHealth(modificationIntentModel);
        }

        public override void ReactToDamage(AttackType attackType)
        {
            aiController.Aggravate();
            if (attackType != AttackType.DoT)
            {
                aiController.ProcessHit();
            }
            base.ReactToDamage(attackType);
        }

        public void SetHealthStats(float maxHealth, float defence)
        {
            this.maxHealth = maxHealth;
            this.defence = defence;
        }
    }
}