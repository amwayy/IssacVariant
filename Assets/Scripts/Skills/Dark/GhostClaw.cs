using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostClaw : Skill
{
    [SerializeField] private float opponentHPThreshold = .7f;   // hp百分比超过这个阈值时造成倍数伤害
    [SerializeField] private float damageModifyScaler = 2f;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        int damageMin = baseDamage - damageDelta;
        int damageMax = baseDamage + damageDelta;
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
