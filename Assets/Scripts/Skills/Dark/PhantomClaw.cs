using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomClaw : Skill
{
    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);
    }
}
