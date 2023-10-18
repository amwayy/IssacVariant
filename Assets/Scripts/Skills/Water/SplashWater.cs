using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashWater : Skill
{
    [SerializeField] private int thisActionPointExpense = 1;
    [SerializeField] private int damageMin = 45;
    [SerializeField] private int damageMax = 55;
    [SerializeField] private int atkDownCountdownMax = 2;
    [SerializeField] private float thisCastTime = 1f;
    [SerializeField] private float setDebuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private Transform atkDownDebuffPrefab;

    private void Awake()
    {
        skillName = "Splash Water";
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(damageMin, damageMax);

        SetAtkDownDebuff();
    }

    private void SetAtkDownDebuff()
    {
        bool isInAtkDown = false;
        AtkDown atkDown = null;
        Transform debuffContainerTransform = skillCaster.GetOpponent().GetDebuffContainerTransform();
        foreach (Transform debuffTransform in debuffContainerTransform)
        {
            if (debuffTransform.TryGetComponent(out atkDown))
            {
                isInAtkDown = true;
                break;
            }
        }

        if (isInAtkDown)
        {
            atkDown.IncreaseCountdown(atkDownCountdownMax);
        }
        else
        {
            skillCaster.GetOpponent().SetDebuff(atkDownDebuffPrefab, atkDownCountdownMax, setDebuffTimerMax);
        }
    }
}
