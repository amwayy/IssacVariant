using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Debuff
{
    [SerializeField] private int damageDelta = 5;

    private int baseDamage;

    public void SetDamage(int damage)
    {
        baseDamage = damage;
    }

    protected override void TurnManager_OnEnterEnemyTurn(object sender, EventArgs e)
    {
        if (!debuffOwner.IsPlayer())
        {
            DecreaseCountdown();
        }
    }

    protected override void TurnManager_OnEnterPlayerTurn(object sender, EventArgs e)
    {
        if (debuffOwner.IsPlayer())
        {
            DecreaseCountdown();
        }
    }

    protected override void DecreaseCountdown()
    {
        base.DecreaseCountdown();

        if (countdown == 0)
        {
            MakeEffect();
        }
    }

    public override void MakeEffect()
    {
        int damage = UnityEngine.Random.Range(baseDamage - damageDelta, baseDamage + damageDelta + 1);
        debuffOwner.TakeDamage(damage);
    }
}
