using System;
using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.System.Combat.Model;

namespace HeroesFlight.System.Combat
{
    public interface CombatSystemInterface : SystemInterface
    {
        event Action<EntityDamageReceivedModel> OnEntityReceivedDamage;
        event Action<EntityDeathModel> OnEntityDied;
        void RegisterEntity(CombatEntityModel model);
        void RevivePlayer();
        void InitCharacterUltimate(List<AnimationData> animations, int charges);
        void UseCharacterUltimate(Action onBeforeUse=null,Action onComplete = null);
        void StartCharacterComboCheck();
    
    }
}