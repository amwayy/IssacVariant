using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AtkUp : Buff
{
    private float atkUpPercentage;
    private int lastAtk;

    public void SetAtkUpPercentage(float atkUpPercentage)
    {
        this.atkUpPercentage = atkUpPercentage;

        MakeEffect();
    }

    public override void MakeEffect()
    {
        lastAtk = buffOwner.GetATK();
        buffOwner.SetATK((int)(lastAtk * (1 + atkUpPercentage)));
    }

    protected override void CheckCountdown()
    {
        if (countdown == 0)
        {
            buffOwner.SetATK(lastAtk);
        }

        base.CheckCountdown();
    }
}
