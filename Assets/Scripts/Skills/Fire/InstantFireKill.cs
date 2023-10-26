using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantFireKill : Skill
{
    //¹¥»÷Á¦=90 ;·ÀÓùÁ¦=40 ; ÑªÁ¿=50
    [SerializeField] private int baseDamage = 90;
    [SerializeField] private int damageDelta = 5;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);

    }
}
