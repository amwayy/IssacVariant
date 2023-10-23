using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightShieldBuff : Shield
{
    [SerializeField] private float backDamagePercentage = .5f;

    public override void MakeEffect()
    {
        base.MakeEffect();

        buffOwner.GetOpponent().TakeDamage((int)(damageTaken * backDamagePercentage));
    }
}
