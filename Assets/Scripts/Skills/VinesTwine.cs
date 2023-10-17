using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VinesTwine : Skill
{
    [SerializeField] private int thisActionPointExpense = 2;
    [SerializeField] private int damageMin = 95;
    [SerializeField] private int damageMax = 105;
    [SerializeField] private float thisCastTime = 1f;
    [SerializeField] private float imprisonProbability = .2f;
    [SerializeField] private float setDebuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private Transform imprisonDebuffPrefab;

    private void Awake()
    {
        skillName = "Vines Twine";
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(damageMin, damageMax);

        System.Random random = new System.Random();
        float randomNum = (float)random.NextDouble();
        if (randomNum <= imprisonProbability)
        {
            SetImprisonDebuff();
        }
    }

    private void SetImprisonDebuff()
    {
        bool isInImprison = false;
        Imprison imprison = null;
        Transform debuffContainerTransform = skillCaster.GetOpponent().GetDebuffContainerTransform();
        foreach (Transform debuffTransform in debuffContainerTransform)
        {
            if (debuffTransform.TryGetComponent(out imprison))
            {
                isInImprison = true;
                break;
            }
        }

        if (isInImprison)
        {
            imprison.IncreaseCountdown(1);
        } else
        {
            skillCaster.GetOpponent().SetDebuff(imprisonDebuffPrefab, 1, setDebuffTimerMax);
        }
    }
}
