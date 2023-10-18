using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBullet : Skill
{
    [SerializeField] private int thisActionPointExpense = 1;
    [SerializeField] private int damageMin = 55;
    [SerializeField] private int damageMax = 65;
    [SerializeField] private float thisCastTime = 1f;

    private void Awake()
    {
        skillName = "Light Bullet";
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(damageMin, damageMax);
    }
}
