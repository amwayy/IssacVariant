using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ForestBlessBuff : Buff
{
    [SerializeField] private float hpPercentage = .1f;   // 伤害不超过最大HP百分比

    protected override void Start()
    {
        base.Start();

        buffOwner.OnAttacked += BuffOwner_OnAttacked;
    }

    private void BuffOwner_OnAttacked(object sender, ISkillCaster.OnAttackedEventArgs e)
    {
        int originalDamage = buffOwner.GetDamageTaken();
        int modifiedDamage = Math.Min((int)(buffOwner.GetHPMaxAmount() * hpPercentage), originalDamage);
        float modifyPercentage = (float)modifiedDamage / originalDamage - 1;
        buffOwner.ModifyDamageTaken(modifyPercentage);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        buffOwner.OnAttacked -= BuffOwner_OnAttacked;
    }
}
