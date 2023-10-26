using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Buff
{
    [SerializeField] private float shieldModifier;   // 伤害变成原来的百分之多少

    protected int damageTaken;

    protected override void Start()
    {
        base.Start();
        buffOwner.OnAttacked += BuffOwner_OnAttacked;
    }

    private void BuffOwner_OnAttacked(object sender, ISkillCaster.OnAttackedEventArgs e)
    {
        damageTaken = e.damage;
        if (!e.isRealDamage)
        {
            MakeEffect();
        }
    }

    public override void MakeEffect()
    {
        buffOwner.ModifyDamageTaken(shieldModifier - 1);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        buffOwner.OnAttacked -= BuffOwner_OnAttacked;
    }
}
