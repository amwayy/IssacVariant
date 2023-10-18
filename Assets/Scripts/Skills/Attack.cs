using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Skill
{
    [SerializeField] private int thisActionPointExpense = 1;
    [SerializeField] private int damageMin = 70;
    [SerializeField] private int damageMax = 80;
    [SerializeField] private float thisCastTime = 1f;

    private void Awake()
    {
        skillName = "Attack";
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(damageMin, damageMax);
    }
}
