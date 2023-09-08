using HeroesFlight.System.Gameplay.Model;
using Spine.Unity;
using StansAssets.Foundation.Async;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class BossCrystalsHealthController : HealthController,BossHealthControllerInterface
    {
        [SerializeField] Collider2D hitCollider;
        [Header("Spine references")]
        [SerializeField] AnimationReferenceAsset deathAniamtion;
        [SerializeField] AnimationReferenceAsset hitAnimation;
        [SerializeField] SkeletonAnimation skeletonAnimation;
        [Header("Visuals")]
        [SerializeField] Color flashColor = Color.gray;
        [Header("Do not modify")]
        [SerializeField] string fillPhaseProperty = "_FillPhase";
        [SerializeField] string fillColorProperty = "_FillColor";

        MaterialPropertyBlock mpb;
        MeshRenderer meshRenderer;

        public override void Init()
        {
            base.Init();
            mpb = new MaterialPropertyBlock();
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        public void SetDefence(float value)
        {
            defence = value;
        }

        public override void DealDamage(DamageModel damage)
        {
            base.DealDamage(damage);
            skeletonAnimation.AnimationState.SetAnimation(1, hitAnimation, false);
            skeletonAnimation.AnimationState.AddEmptyAnimation(1, .2f, 0);
        }

        protected override void ProcessDeath()
        {
            base.ProcessDeath();
            skeletonAnimation.AnimationState.SetEmptyAnimation(1, .5f);
            skeletonAnimation.AnimationState.SetAnimation(0, deathAniamtion, false);
            CoroutineUtility.WaitForSeconds(deathAniamtion.Animation.Duration, () =>
            {
                gameObject.SetActive(false);
            });
        }

        public override void SetInvulnerableState(bool isImmortal)
        {
            base.SetInvulnerableState(isImmortal);
            hitCollider.enabled = !isImmortal;
            Flash(isImmortal);
        }


         void Flash (bool isInvul)
         {
            meshRenderer.GetPropertyBlock(mpb);
            var fillPhase = Shader.PropertyToID(fillPhaseProperty);
            var fillColor = Shader.PropertyToID(fillColorProperty);
            mpb.SetColor(fillColor, flashColor);
            var fillValue = isInvul ? 1 : 0;
            mpb.SetFloat(fillPhase,fillValue );
            meshRenderer.SetPropertyBlock(mpb);
        }
         
         

    }
}