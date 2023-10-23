using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedHeal : Buff
{
    private float healPercentage;

    public void SetHealPercentage(float healPercentage)
    {
        this.healPercentage = healPercentage;
    }

    public override void MakeEffect()
    {
        buffOwner.Heal((int)(buffOwner.GetHPMaxAmount() * healPercentage));
    }

    protected override void TurnManager_OnEnterPlayerTurn(object sender, EventArgs e)
    {
        MakeEffect();

        base.TurnManager_OnEnterPlayerTurn(sender, e);
    }

    protected override void TurnManager_OnEnterEnemyTurn(object sender, EventArgs e)
    {
        MakeEffect();

        base.TurnManager_OnEnterEnemyTurn(sender, e);
    }
}
