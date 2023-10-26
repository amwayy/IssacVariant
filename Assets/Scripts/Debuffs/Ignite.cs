using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ignite : Anomaly
{
    [SerializeField] private float hpMaxModifyPercentage = .875f;

    public override void MakeEffect()
    {
        debuffOwner.ModifyHPMaxAmount((int)(debuffOwner.GetHPMaxAmount() * hpMaxModifyPercentage));
    }
}
