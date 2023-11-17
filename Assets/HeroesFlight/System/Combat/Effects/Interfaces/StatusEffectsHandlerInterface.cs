﻿using HeroesFlight.System.Combat.Effects.Effects;

namespace HeroesFlight.System.Combat.StatusEffects
{
    public interface StatusEffectsHandlerInterface
    {
        void ApplyStatusEffect(StatusEffect effect);
        void ExecuteTick();
    }
}