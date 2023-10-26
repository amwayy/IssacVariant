using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestBless : Skill
{
    [SerializeField] private float healPercentage = .3f;
    [SerializeField] private int forestBlessBuffCountdownMax = 1;
    [SerializeField] private float setBuffTimerMax = .5f;
    [SerializeField] private Transform forestBlessBuffPrefab;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.Heal((int)(skillCaster.GetHPMaxAmount() * healPercentage));

        SetForestBlessBuff();
    }

    private void SetForestBlessBuff()
    {
        bool isInForestBless = false;
        ForestBlessBuff forestBlessBuff = null;
        Transform buffContainerTransform = skillCaster.GetBuffContainerTransform();
        foreach (Transform buffTransform in buffContainerTransform)
        {
            if (buffTransform.TryGetComponent(out forestBlessBuff))
            {
                isInForestBless = true;
                break;
            }
        }

        if (isInForestBless)
        {
            forestBlessBuff.IncreaseCountdown(forestBlessBuffCountdownMax);
        }
        else
        {
            skillCaster.SetBuff(forestBlessBuffPrefab, forestBlessBuffCountdownMax, setBuffTimerMax);
        }
    }
}
