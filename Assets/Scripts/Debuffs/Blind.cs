using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blind : Anomaly
{
    [SerializeField] private float missProbability = .75f;

    protected override void Start()
    {
        base.Start();

        debuffOwner.OnAttackReady += DebuffOwner_OnAttackReady;
    }

    protected override void TurnManager_OnEnterEnemyTurn(object sender, EventArgs e)
    {
        if (debuffOwner.IsPlayer())
        {
            DecreaseCountdown();
        }
    }

    protected override void TurnManager_OnEnterPlayerTurn(object sender, EventArgs e)
    {
        if (!debuffOwner.IsPlayer())
        {
            DecreaseCountdown();
        }
    }

    private void DebuffOwner_OnAttackReady(object sender, System.EventArgs e)
    {
        System.Random random = new System.Random();
        float randomNum = (float)random.NextDouble();
        if (randomNum < missProbability)
        {
            debuffOwner.ModifyAttackDamage(-1);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        debuffOwner.OnAttackReady -= DebuffOwner_OnAttackReady;
    }
}
