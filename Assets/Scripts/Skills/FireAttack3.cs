using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttack3 : Skill
{
    //¹¥»÷Á¦=90 ;·ÀÓùÁ¦=40 ; ÑªÁ¿=50
    [SerializeField] private int thisActionPointExpense = 1;
    [SerializeField] private int damageMin = 95;
    [SerializeField] private int damageMax = 85;
    [SerializeField] private float thisCastTime = 1f;

    private void Awake()
    {
        skillName = "Fire Attack3";
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(damageMin, damageMax);

    }
}
