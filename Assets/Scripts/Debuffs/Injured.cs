using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Injured : Debuff
{
    // 每回合受到百分比伤害
    private float damagePercentage;

    public void SetDamagePercentage(float damagePercentage)
    {
        this.damagePercentage = damagePercentage;
        MakeEffect();
        DecreaseCountdown();
    }

    public override void MakeEffect()
    {
        debuffOwner.TakeDamage((int)(debuffOwner.GetHPAmount() * damagePercentage));
    }

    protected override void TurnManager_OnEnterPlayerTurn(object sender, EventArgs e)
    {
        if (debuffOwner.IsPlayer())
        {
            MakeEffect();
            DecreaseCountdown();
        }
    }

    protected override void TurnManager_OnEnterEnemyTurn(object sender, EventArgs e)
    {
        if (!debuffOwner.IsPlayer())
        {
            MakeEffect();
            DecreaseCountdown();
        }
    }
}
