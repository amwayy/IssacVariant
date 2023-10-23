using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostClaw : Skill
{
    [SerializeField] private int thisActionPointExpense = 1;
    [SerializeField] private int damageMin = 75;
    [SerializeField] private int damageMax = 85;
    [SerializeField] private float thisCastTime = 1f;
    [SerializeField] private float opponentHPThreshold = .7f;   // hp百分比超过这个阈值时造成倍数伤害
    [SerializeField] private float damageModifyScaler = 2f;

    private void Awake()
    {
        skillName = "Ghost Claw";   // 幽爪
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        ISkillCaster opponent = skillCaster.GetOpponent();
        if (opponent.GetHPAmount() >= opponentHPThreshold * opponent.GetHPMaxAmount())
        {
            skillCaster.SetAttack((int)(damageMin * damageModifyScaler), (int)(damageMax * damageModifyScaler));
        } else
        {
            skillCaster.SetAttack(damageMin, damageMax);
        }
    }
}
