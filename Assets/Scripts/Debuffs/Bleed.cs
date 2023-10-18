using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleed : Anomaly
{
    [SerializeField] private int bleedDamage = 20;

    public override void MakeEffect()
    {
        debuffOwner.TakeDamage(bleedDamage);
    }
}
