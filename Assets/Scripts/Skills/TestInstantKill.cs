using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInstantKill : Skill
{
    [SerializeField] private int thisActionPointExpense = 1;
    [SerializeField] private float thisCastTime = 1f;

    private void Awake()
    {
        skillName = "Test Instant Kill";
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        int damage = skillCaster.GetOpponent().GetHPMaxAmount();
        skillCaster.SetAttack(damage, damage);
    }
}
