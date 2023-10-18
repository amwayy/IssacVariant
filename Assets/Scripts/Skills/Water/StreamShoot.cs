using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamShoot : Skill
{
    [SerializeField] private int thisActionPointExpense = 1;
    [SerializeField] private int damageMin = 65;
    [SerializeField] private int damageMax = 75;
    [SerializeField] private float thisCastTime = 1f;

    private void Awake()
    {
        skillName = "Stream Shoot";
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(damageMin, damageMax);
    }
}
