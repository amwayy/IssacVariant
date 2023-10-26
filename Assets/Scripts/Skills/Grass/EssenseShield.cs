using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenseShield : Skill
{
    [SerializeField] private int halfShieldCountdownMax = 2;
    [SerializeField] private float setBuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private Transform halfShieldBuffPrefab;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        SetHalfShieldBuff();
    }

    private void SetHalfShieldBuff()
    {
        bool isInHalfShield = false;
        HalfShield halfShield = null;
        Transform buffContainerTransform = skillCaster.GetBuffContainerTransform();
        foreach (Transform buffTransform in buffContainerTransform)
        {
            if (buffTransform.TryGetComponent(out halfShield))
            {
                isInHalfShield = true;
                break;
            } else if (buffTransform.TryGetComponent(out Shield shield))
            {
                shield.DestroySelf();
            }
        }

        if (isInHalfShield)
        {
            halfShield.IncreaseCountdown(halfShieldCountdownMax);
        }
        else
        {
            skillCaster.SetBuff(halfShieldBuffPrefab, halfShieldCountdownMax, setBuffTimerMax);
        }
    }
}
