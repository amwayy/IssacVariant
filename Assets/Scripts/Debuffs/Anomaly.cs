using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anomaly : Debuff
{
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
