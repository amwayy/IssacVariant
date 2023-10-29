using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveFire :Skill
{
    [SerializeField] private float healthMin = .3f;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);
        if(skillCaster.GetHPAmount() < skillCaster.GetHPMaxAmount() * healthMin)
        {
            //»ðÊôÐÔÉËº¦¶îÍâ+20%

        }
    }
}
