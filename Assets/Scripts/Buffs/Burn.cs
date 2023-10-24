using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : Buff
{
    private float appendDamagePercentage = .2f;   // 追加攻击是已损失血量的百分之多少

    protected override void Start()
    {
        base.Start();

        buffOwner.OnAttackReady += BuffOwner_OnAttackReady;
    }

    private void BuffOwner_OnAttackReady(object sender, System.EventArgs e)
    {
        int lostHP = buffOwner.GetHPMaxAmount() - buffOwner.GetHPAmount();
        int appendDamage = (int)(lostHP * appendDamagePercentage);
        buffOwner.AppendDamage(appendDamage);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        buffOwner.OnAttackReady -= BuffOwner_OnAttackReady;
    }
}
