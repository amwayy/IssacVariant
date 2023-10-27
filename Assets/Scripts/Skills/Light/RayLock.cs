using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayLock : Skill
{
    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta, isRealDamage: true);
    }
}
