using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFire : Skill
{
    //¹¥»÷Á¦=70 ;·ÀÓùÁ¦=30 ; ÑªÁ¿=50
    [SerializeField] private int baseDamage = 70;
    [SerializeField] private int damageDelta = 5;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);

    }
}
