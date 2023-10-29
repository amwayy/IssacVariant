using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInstantKill : Skill
{
    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        int damage = skillCaster.GetOpponent().GetHPMaxAmount() * 100;
        skillCaster.SetAttack(damage, damage);
    }
}
