using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpLeaf : Skill
{
    [SerializeField] private int thisActionPointExpense = 1;
    [SerializeField] private int damageMin = 70;
    [SerializeField] private int damageMax = 80;
    [SerializeField] private int bleedCountdownMax = 4;
    [SerializeField] private float thisCastTime = 1f;
    [SerializeField] private float bleedProbability = .2f;
    [SerializeField] private float setDebuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private Transform bleedDebuffPrefab;

    private void Awake()
    {
        skillName = "Sharp Leaf";
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(damageMin, damageMax);

        System.Random random = new System.Random();
        float randomNum = (float) random.NextDouble();
        if (randomNum <= bleedProbability)
        {
            SetBloodDebuff();
        }
    }

    private void SetBloodDebuff()
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
