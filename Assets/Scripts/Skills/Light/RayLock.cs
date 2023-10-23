using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayLock : Skill
{
    [SerializeField] private int thisActionPointExpense = 1;
    [SerializeField] private int damageMin = 50;
    [SerializeField] private int damageMax = 60;
    [SerializeField] private float thisCastTime = 1f;

    private void Awake()
    {
        skillName = "Ray Lock";
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(damageMin, damageMax, isRealDamage: true);
    }
}
