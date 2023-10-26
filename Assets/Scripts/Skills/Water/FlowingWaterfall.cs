using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowingWaterfall : Skill
{
    [SerializeField] private int baseDamage = 20;
    [SerializeField] private int damageDelta = 5;
    [SerializeField] private int minAttackCount = 2;
    [SerializeField] private int maxAttackCount = 4;
    [SerializeField] private int attackModifyAmount = 15;
    [SerializeField] private float attackSpeed = 20f;
    [SerializeField] private float singleCastTime = 1f;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        int randomAttackCount = Random.Range(minAttackCount, maxAttackCount + 1);
        castTime = singleCastTime * randomAttackCount;

        base.CastSkill(skillCaster);

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta, attackSpeed, randomAttackCount);
        skillCaster.SetAttackModify(attackModifyAmount);
    }
}
