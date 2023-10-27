using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowingLight : Skill
{
    [SerializeField] private float instantKillProbability = .05f;
    [SerializeField] private int enemyDamageModifyAmount = 10;
    [SerializeField] private int instantKillFactor = 100;   // 防止对方减伤效果，秒杀伤害为对方HP上限的x倍

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        if (skillCaster.IsPlayer())
        {
            System.Random random = new System.Random();
            float randomNum = (float)random.NextDouble();
            if (randomNum < instantKillProbability)
            {
                int instantKillDamage = skillCaster.GetOpponent().GetHPMaxAmount() * instantKillFactor;
                skillCaster.SetAttack(instantKillDamage, instantKillDamage, isRealDamage: true);
            } else
            {
                skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);
            }
        } else
        {
            skillCaster.SetAttack(baseDamage + enemyDamageModifyAmount - damageDelta, baseDamage + enemyDamageModifyAmount + damageDelta);
        }
    }
}
