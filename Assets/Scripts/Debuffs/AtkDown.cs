using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkDown : Debuff
{
    [SerializeField] private float atkDownPercentage = .2f;

    private int lastAtk;

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

    public override void Initialize(ISkillCaster skillCaster, int countdown, float setDebuffTimerMax, int extraCountdown)
    {
        base.Initialize(skillCaster, countdown, setDebuffTimerMax, extraCountdown);

        MakeEffect();
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
