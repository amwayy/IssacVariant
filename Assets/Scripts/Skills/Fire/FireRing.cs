using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRing : Skill
{
    [SerializeField] private int igniteCountdownMax = 2; //灼烧状态 持续2回合
    [SerializeField] private int doubleDamageCountdownMax = 1; //
    [SerializeField] private float igniteProbability = .25f;
    [SerializeField] private float setBuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private float setDebuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private Transform igniteDebuffPrefab;
    [SerializeField] private Transform countDoubleDamageBuffPrefab;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);

        System.Random random = new System.Random();
        float randomNum = (float)random.NextDouble();
        if (randomNum <= igniteProbability) //灼烧
        {
            SetIgniteDebuff();
        }

        SetCountDoubleDamageBuff();
    }

    //灼烧
    private void SetIgniteDebuff()
    {
        bool isInIgnite = false; //
        Ignite ignite = null;
        Transform debuffContainerTransform = skillCaster.GetOpponent().GetDebuffContainerTransform();
        foreach (Transform debuffTransform in debuffContainerTransform)
        {
            if (debuffTransform.TryGetComponent(out ignite))
            {
                isInIgnite = true;
                break;
            }
        }

        if (isInIgnite)
        {
            ignite.IncreaseCountdown(igniteCountdownMax);
        }
        else
        {
            skillCaster.GetOpponent().SetDebuff(igniteDebuffPrefab, igniteCountdownMax, setDebuffTimerMax);
        }
    }

    private void SetCountDoubleDamageBuff()
    {
        bool isInDoubleDamage = false;
        CountDoubleDamage doubleDamage = null;
        Transform buffContainerTransform = skillCaster.GetBuffContainerTransform();

        foreach (Transform buffTransform in buffContainerTransform)
        {
            if (buffTransform.TryGetComponent(out doubleDamage))
            {
                isInDoubleDamage = true;
                break;
            }
        }

        if (isInDoubleDamage)
        {
            doubleDamage.IncreaseCountdown(doubleDamageCountdownMax);
        }
        else
        {
            skillCaster.SetBuff(countDoubleDamageBuffPrefab, doubleDamageCountdownMax, setBuffTimerMax);
        }
    }
}
