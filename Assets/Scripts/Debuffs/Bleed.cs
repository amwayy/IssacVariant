using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleed : Debuff
{
    [SerializeField] private int bleedDamage = 20;

    public override void MakeEffect()
    {
        skillCaster.TakeDamage(bleedDamage);

        base.MakeEffect();
    }
}
