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
        buffOwner.OnCheckShield += BuffOwner_OnCheckShield;
    }

    private void BuffOwner_OnCheckShield(object sender, int e)
    {
        damageTaken = e;
        MakeEffect();
    }

    public override void MakeEffect()
    {
        buffOwner.SetDamageTaken((int)(damageTaken * shieldModifier));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        buffOwner.OnCheckShield -= BuffOwner_OnCheckShield;
    }
}
