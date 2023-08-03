using HeroesFlight.System.Character;


namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CharacterHealthController : HealthController
    {
        CharacterControllerInterface controller;
        CharacterAttackController attackController;
        CharacterAnimationControllerInterface animator;

        public override void Init()
        {
            controller = GetComponent<CharacterControllerInterface>();
            attackController = GetComponent<CharacterAttackController>();
            animator.PlayIdleAnimation();
            animator = GetComponent<CharacterAnimationController>();
            maxHealth = controller.CharacterSO.GetPlayerStatData.Health;
            base.Init();
        }

        protected override void ProcessDeath()
        {
            controller.SetActionState(false);
            attackController.ToggleControllerState(false);
            animator.PlayDeathAnimation();
            base.ProcessDeath();
        }

        public override void Reset()
        {
            controller.SetActionState(true);
            attackController.ToggleControllerState(true);
            animator.PlayDeathAnimation();
            base.Reset();
        }

        public override void Revive()
        {
            controller.SetActionState(true);
            attackController.ToggleControllerState(true);
            animator.PlayDeathAnimation();
            base.Revive();
        }

       
    }
}