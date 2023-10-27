using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallucinationClaw : Skill
{
    [SerializeField] private int attackModifyAmount = 30;
    [SerializeField] private float attackSpeed = 120f;
    [SerializeField] private int bleedCountdownMax = 4;
    [SerializeField] private float bleedProbability = 1f;
    [SerializeField] private float setDebuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private Transform bleedDebuffPrefab;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);
        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta, attackSpeed);
        skillCaster.SetAttackModify(attackModifyAmount);
        SetBleedDebuff(); //100%会流血
    }
    private void SetBleedDebuff()
    {
        bool isInBleed = false;
        Bleed bleed = null;
        Transform debuffContainerTransform = skillCaster.GetOpponent().GetDebuffContainerTransform();
        foreach (Transform debuffTransform in debuffContainerTransform)
        {
            if (debuffTransform.TryGetComponent(out bleed))
            {
                isInBleed = true;
                break;
            }
        }

        if (isInBleed)
        {
            bleed.IncreaseCountdown(bleedCountdownMax);
        }
        else
        {
            skillCaster.GetOpponent().SetDebuff(bleedDebuffPrefab, bleedCountdownMax, setDebuffTimerMax);
        }
    }
}
