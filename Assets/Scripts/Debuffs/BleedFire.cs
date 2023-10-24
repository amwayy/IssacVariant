using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedFire : Anomaly
{
    [SerializeField] private float bleedLoss = .125f;

    public override void MakeEffect()
    {
        int bleedDamage = (int)(debuffOwner.GetHPMaxAmount() * .125f);
        debuffOwner.TakeDamage(bleedDamage);
    }
}
