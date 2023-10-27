using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareBuff : Buff
{
    [SerializeField] private float perExpenseDamageIncreasePercentage = .1f;

    protected override void Start()
    {
        base.Start();

        buffOwner.OnAttackReady += BuffOwner_OnAttackReady;
    }

    private void BuffOwner_OnAttackReady(object sender, System.EventArgs e)
    {
        Skill castSkill = buffOwner.GetLastCastSkill();
        int actionPointExpense = castSkill.GetActionPointExpense();
        buffOwner.ModifyAttackDamage(perExpenseDamageIncreasePercentage * actionPointExpense);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        buffOwner.OnAttackReady -= BuffOwner_OnAttackReady;
    }
}
