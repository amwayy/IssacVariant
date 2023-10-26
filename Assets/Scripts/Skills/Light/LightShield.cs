using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightShield : Skill
{
    [SerializeField] private int lightShieldCountdownMax = 1;
    [SerializeField] private float setBuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private Transform lightShieldBuffPrefab;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        SetLightShieldBuff();
    }

    private void SetLightShieldBuff()
    {
        bool isInLightShield = false;
        LightShieldBuff lightShield = null;
        Transform buffContainerTransform = skillCaster.GetBuffContainerTransform();
        foreach (Transform buffTransform in buffContainerTransform)
        {
            if (buffTransform.TryGetComponent(out lightShield))
            {
                isInLightShield = true;
            }
            else if (buffTransform.TryGetComponent(out Shield shield))
            {
                shield.DestroySelf();
            }

        }

        if (isInLightShield)
        {
            lightShield.IncreaseCountdown(lightShieldCountdownMax);
        }
        else
        {
            skillCaster.SetBuff(lightShieldBuffPrefab, lightShieldCountdownMax, setBuffTimerMax);
        }
    }
}
