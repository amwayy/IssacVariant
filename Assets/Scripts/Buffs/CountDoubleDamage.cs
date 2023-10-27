using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDoubleDamage : Buff
{
    [SerializeField] private float damageIncreasePercentage = 1f;

    protected override void Start()
    {
        base.Start();

        buffOwner.OnAttackReady += BuffOwner_OnAttackReady;
    }

    protected override void TurnManager_OnEnterEnemyTurn(object sender, EventArgs e)
    {

    }

    protected override void TurnManager_OnEnterPlayerTurn(object sender, EventArgs e)
    {

    }

    private void BuffOwner_OnAttackReady(object sender, System.EventArgs e)
    {
        buffOwner.ModifyAttackDamage(damageIncreasePercentage);

        DecreaseCountdown();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        buffOwner.OnAttackReady -= BuffOwner_OnAttackReady;
    }
}
