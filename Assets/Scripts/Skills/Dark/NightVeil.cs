using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightVeil : Skill
{
    [SerializeField] private int thisActionPointExpense = 2;
    [SerializeField] private int delayedHealCountdownMax = 2;
    [SerializeField] private float thisCastTime = 1f;
    [SerializeField] private float setBuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private Transform delayedHealDebuffPrefab;
    [SerializeField] private float healPercentage = .15f;

    private void Awake()
    {
        skillName = "Night Veil";   // 夜幕
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        SetDelayedHealBuff();
    }

    private void SetDelayedHealBuff()
    {
        bool isInDelayedHeal = false;
        DelayedHeal delayedHeal = null;
        Transform buffContainerTransform = skillCaster.GetBuffContainerTransform();
        foreach (Transform buffTransform in buffContainerTransform)
        {
            if (buffTransform.TryGetComponent(out delayedHeal))
            {
                isInDelayedHeal = true;
                break;
            }
        }

        if (isInDelayedHeal)
        {
            delayedHeal.IncreaseCountdown(delayedHealCountdownMax);
        }
        else
        {
            Buff buff = skillCaster.SetBuff(delayedHealDebuffPrefab, delayedHealCountdownMax, setBuffTimerMax);
            buff.GetComponent<DelayedHeal>().SetHealPercentage(healPercentage);
        }
    }
}
