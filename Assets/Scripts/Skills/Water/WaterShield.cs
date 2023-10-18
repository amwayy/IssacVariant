using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterShield : Skill
{
    [SerializeField] private int thisActionPointExpense = 2;
    [SerializeField] private int waterShieldCountdownMax = 2;
    [SerializeField] private int damageMin = 65;
    [SerializeField] private int damageMax = 75;
    [SerializeField] private float thisCastTime = 1f;
    [SerializeField] private float setBuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private Transform waterShieldBuffPrefab;

    private void Awake()
    {
        skillName = "Water Shield";
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(damageMin, damageMax);

        SetWaterShieldBuff();
    }

    private void SetWaterShieldBuff()
    {
        bool isInWaterShield = false;
        WaterShieldBuff waterShield = null;
        Transform buffContainerTransform = skillCaster.GetBuffContainerTransform();
        foreach (Transform buffTransform in buffContainerTransform)
        {
            if (buffTransform.TryGetComponent(out waterShield))
            {
                isInWaterShield = true;
            } else if (buffTransform.TryGetComponent(out Shield shield))
            {
                shield.DestroySelf();
            }

        }

        if (isInWaterShield)
        {
            waterShield.IncreaseCountdown(waterShieldCountdownMax);
        }
        else
        {
            skillCaster.SetBuff(waterShieldBuffPrefab, waterShieldCountdownMax, setBuffTimerMax);
        }
    }
}
