using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttack2 : Skill
{
    [SerializeField] private int thisActionPointExpense = 2;
    [SerializeField] private int damageMin = 45;
    [SerializeField] private int damageMax = 55;
    [SerializeField] private int burnCountdownMax = 2; //灼烧状态 持续2回合
    [SerializeField] private int doubleAttackCountdownMax = 1; //
    [SerializeField] private float thisCastTime = 1f;
    [SerializeField] private float burnProbability = .25f;
    [SerializeField] private float setBuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private float setDebuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private Transform bleedDebuffPrefab;
    [SerializeField] private Transform doubleAttackBuffPrefab;

    private void Awake()
    {
        skillName = "Fire Attack2";
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(damageMin, damageMax);

        System.Random random = new System.Random();
        float randomNum = (float)random.NextDouble();
        if (randomNum <= burnProbability) //灼烧
        {
            SetBurnDebuff();
        }
        //下一轮伤害加倍
        setDoubleAttack();
    }
    //上面完成
    private void SetBurnDebuff()
    {
        bool isInBurn = false; //
        BleedFire bleed = null;
        Transform debuffContainerTransform = skillCaster.GetOpponent().GetDebuffContainerTransform();
        foreach (Transform debuffTransform in debuffContainerTransform)
        {
            if (debuffTransform.TryGetComponent(out bleed))
            {
                isInBurn = true;
                break;
            }
        }

        if (isInBurn)
        {
            bleed.IncreaseCountdown(burnCountdownMax);
        }
        else
        {
            skillCaster.GetOpponent().SetDebuff(bleedDebuffPrefab, burnCountdownMax, setDebuffTimerMax);
        }
    }

    private void setDoubleAttack()
    {
        bool isInDoubleAttack = false;
        DoubleAttack doubleAttack = null;
        Transform buffContainerTransform = skillCaster.GetBuffContainerTransform();
        foreach (Transform buffTransform in buffContainerTransform)
        {
            if (buffTransform.TryGetComponent(out doubleAttack))
            {
                isInDoubleAttack = true;
                break;
            }
            else if (buffTransform.TryGetComponent(out AddDamage adddamage))
            {
                adddamage.DestroySelf();
            }
        }

        if (isInDoubleAttack)
        {
            doubleAttack.IncreaseCountdown(doubleAttackCountdownMax);
        }
        else
        {
            skillCaster.SetBuff(doubleAttackBuffPrefab, doubleAttackCountdownMax, setBuffTimerMax);
        }
    }
}
