using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDamage : Buff
{
    private float damageModifyPercentage = 2f;

    protected override void Start()
    {
        base.Start();

        buffOwner.OnAttackReady += BuffOwner_OnAttackReady;
    }

    private void BuffOwner_OnAttackReady(object sender, System.EventArgs e)
    {
        if (buffOwner.GetLastCastSkill().GetElement() == GameLibrary.Element.Water)
        {
            buffOwner.ModifyAttackDamage(damageModifyPercentage - 1);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        buffOwner.OnAttackReady -= BuffOwner_OnAttackReady;
    }
}
