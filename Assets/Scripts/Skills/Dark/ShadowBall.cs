using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBall : Skill
{
    [SerializeField] private int thisActionPointExpense = 1;
    [SerializeField] private int damageMin = 60;
    [SerializeField] private int damageMax = 70;
    [SerializeField] private float thisCastTime = 1f;

    private void Awake()
    {
        skillName = "Shadow Ball";
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(damageMin, damageMax);
    }
}
