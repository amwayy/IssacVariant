using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBullet : Skill
{
    [SerializeField] private int baseDamage = 60;
    [SerializeField] private int damageDelta = 5;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);
    }
}
