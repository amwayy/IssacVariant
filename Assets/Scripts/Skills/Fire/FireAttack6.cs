using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttack6 : Skill
{
    //攻击力=0 ;防御力=50 ; 血量=100
    [SerializeField] private int thisActionPointExpense = 2;
    //持续两回合
    [SerializeField] private int round = 2;
    [SerializeField] private float thisCastTime = 1f;
    //每回合失去当前血量的15%
    [SerializeField] private float loseBlood = .15f;

    private void Awake()
    {
        skillName = "Fire Attack6";
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);
        for (int i = 0; i < 2; i++)
        {
            int damage = (int)(skillCaster.GetHPAmount() * .15f);
            skillCaster.TakeDamage(damage);
        }
    }
}
