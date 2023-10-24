using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefUp : Buff
{
    private float defUpPercentage;
    private int lastDef;

    public void SetDefUpPercentage(float defUpPercentage)
    {
        this.defUpPercentage = defUpPercentage;

        MakeEffect();
    }

    public override void MakeEffect()
    {
        lastDef = buffOwner.GetDEF();
        buffOwner.SetDEF((int)(lastDef * (1 + defUpPercentage)));
    }

    protected override void CheckCountdown()
    {
        if (countdown == 0)
        {
            buffOwner.SetDEF(lastDef);
        }

        base.CheckCountdown();
    }
}
