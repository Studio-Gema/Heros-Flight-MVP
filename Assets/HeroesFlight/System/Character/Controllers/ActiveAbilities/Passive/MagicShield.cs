using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShield : PassiveActiveAbility
{
    public override void OnActivated()
    {

    }

    public override void OnCoolDownEnded()
    {

    }

    public override void OnCoolDownStarted()
    {

    }

    public void Initialize(int level)
    {
        this.level = level;
    }
}
