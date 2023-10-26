using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strength : Skill
{
    [SerializeField] private int baseDamage = 90;
    [SerializeField] private int damageDelta = 5;
    [SerializeField] private float atkUpPercentage = .1f;
    [SerializeField] private float defUpPercentage = .1f;
    [SerializeField] private Transform atkUpBuffPrefab;
    [SerializeField] private Transform defUpBuffPrefab;
    [SerializeField] private float setBuffTimerMax = .5f;   // 设定Debuff的缓冲时间

    private int atkUpCountdownMax = 1;
    private int defUpCountdownMax = 1;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        SetAtkUpBuff();
        SetDefUpBuff();
        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);
    }

    private void SetDefUpBuff()
    {
        bool isInDefUp = false;
        DefUp defUp = null;
        Transform buffContainerTransform = skillCaster.GetBuffContainerTransform();
        foreach (Transform buffTransform in buffContainerTransform)
        {
            if (buffTransform.TryGetComponent(out defUp))
            {
                isInDefUp = true;
                break;
            }
        }

        if (isInDefUp)
        {
            defUp.IncreaseCountdown(defUpCountdownMax);
        }
        else
        {
            Buff buff = skillCaster.SetBuff(defUpBuffPrefab, defUpCountdownMax, setBuffTimerMax);
            buff.GetComponent<DefUp>().SetDefUpPercentage(defUpPercentage);
        }
    }

    private void SetAtkUpBuff()
    {
        bool isInAtkUp = false;
        AtkUp atkUp = null;
        Transform buffContainerTransform = skillCaster.GetBuffContainerTransform();
        foreach (Transform buffTransform in buffContainerTransform)
        {
            if (buffTransform.TryGetComponent(out atkUp))
            {
                isInAtkUp = true;
                break;
            }
        }

        if (isInAtkUp)
        {
            atkUp.IncreaseCountdown(atkUpCountdownMax);
        }
        else
        {
            Buff buff = skillCaster.SetBuff(atkUpBuffPrefab, atkUpCountdownMax, setBuffTimerMax);
            buff.GetComponent<AtkUp>().SetAtkUpPercentage(atkUpPercentage);
        }
    }
}
