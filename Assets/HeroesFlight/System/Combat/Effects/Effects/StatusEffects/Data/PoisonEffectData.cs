﻿using System;

namespace HeroesFlight.System.Combat.Effects.Effects.Data
{
    [Serializable]
    public class PoisonEffectData : EffectData
    {
        public EffectValuePair Damage = new EffectValuePair();
    }
}