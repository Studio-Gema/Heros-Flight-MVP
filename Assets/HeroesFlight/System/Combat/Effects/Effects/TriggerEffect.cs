﻿using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [CreateAssetMenu(fileName = "TriggerEffect", menuName = "Combat/Effects/Trigger", order = 100)]
    public class TriggerEffect : Effect
    {
        public override T GetData<T>()
        {
            throw new NotImplementedException();
        }
    }
}