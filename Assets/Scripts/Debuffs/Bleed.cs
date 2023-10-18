using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleed : Debuff
{
    [SerializeField] private int bleedDamage = 20;

    public override void MakeEffect()
    {
        debuffOwner.TakeDamage(bleedDamage);
    }
}
