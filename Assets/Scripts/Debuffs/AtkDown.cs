using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkDown : Debuff
{
    private float atkDownPercentage;
    private int lastAtk;

    public void SetAtkDownPercentage(float atkDownPercentage)
    {
        this.atkDownPercentage = atkDownPercentage;

        MakeEffect();
    }

    protected override void TurnManager_OnEnterPlayerTurn(object sender, EventArgs e)
    {
        if (!debuffOwner.IsPlayer())
        {
            DecreaseCountdown();
        }
    }

    protected override void TurnManager_OnEnterEnemyTurn(object sender, EventArgs e)
    {
        if (debuffOwner.IsPlayer())
        {
            DecreaseCountdown();
        }
    }

    public override void MakeEffect()
    {
        lastAtk = debuffOwner.GetATK();
        debuffOwner.SetATK((int)(lastAtk * (1 - atkDownPercentage)));
    }

    protected override void CheckCountdown()
    {
        if (countdown == 0)
        {
            debuffOwner.SetATK(lastAtk);
        }

        base.CheckCountdown();
    }
}
