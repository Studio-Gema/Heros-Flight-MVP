﻿using System;

namespace HeroesFlight.System.Combat.Effects.Effects.Data
{
    [Serializable]
    public class EffectValuePair
    {
        public float StartValue;
        public float IncreasePerLvl;

        public float GetCurrentValue(int lvl)
        {
            return StartValue + IncreasePerLvl * (lvl);
        }

        public void SetStartValue(float value)
        {
            StartValue = value;
        }
    }
}