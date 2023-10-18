using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowingWaterfall : Skill
{
    [SerializeField] private int thisActionPointExpense = 1;
    [SerializeField] private int singleDamageMin = 15;
    [SerializeField] private int singleDamageMax = 25;
    [SerializeField] private int minAttackCount = 2;
    [SerializeField] private int maxAttackCount = 4;
    [SerializeField] private int attackModifyAmount = 15;
    [SerializeField] private float attackSpeed = 20f;
    [SerializeField] private float singleCastTime = 1f;

    private void Awake()
    {
        skillName = "Flowing Waterfall";
        actionPointExpense = thisActionPointExpense;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        int randomAttackCount = Random.Range(minAttackCount, maxAttackCount + 1);
        castTime = singleCastTime * randomAttackCount;

        base.CastSkill(skillCaster);

        skillCaster.SetAttack(singleDamageMin, singleDamageMax, attackSpeed, randomAttackCount);
        skillCaster.SetAttackModify(attackModifyAmount);
    }
}
