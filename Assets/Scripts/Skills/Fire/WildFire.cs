using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFire : Skill
{
    //¹¥»÷Á¦=70 ;·ÀÓùÁ¦=30 ; ÑªÁ¿=50
    [SerializeField] private int thisActionPointExpense = 1;
    [SerializeField] private int damageMin = 65;
    [SerializeField] private int damageMax = 75;
    [SerializeField] private float thisCastTime = 1f;

    private void Awake()
    {
        skillName = "Wild Fire";   // »Ä»ð
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(damageMin, damageMax);

    }
}
