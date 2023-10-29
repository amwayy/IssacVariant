using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivineFire :Skill
{
    [SerializeField] private int igniteCountdownMax = 2;
    [SerializeField] private int twentyDamageCountdownMax = 1;
    [SerializeField] private float igniteProbability = .5f;
    [SerializeField] private float setBuffTimerMax = .5f;
    [SerializeField] private float setDebuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private Transform igniteDebuffPrefab;
    [SerializeField] private Transform countTwentyDamageBuffPrefab;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);

        System.Random random = new System.Random();
        float randomNum = (float)random.NextDouble();
        if (randomNum <= igniteProbability)
        {
            SetIgniteDebuff();
        }
    }

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
            ignite.IncreaseCountdown(igniteCountdownMax+1);
            //还会下个伤害
            SetTwentyDamageBuff();
        }
        else
        {
            skillCaster.GetOpponent().SetDebuff(igniteDebuffPrefab, igniteCountdownMax, setDebuffTimerMax);
        }
    }
    private void SetTwentyDamageBuff()
    {
        bool isInTwentyDamage = false;
        CountTwentyDamage twentyDamage = null;
        Transform buffContainerTransform = skillCaster.GetBuffContainerTransform();

        foreach (Transform buffTransform in buffContainerTransform)
        {
            if (buffTransform.TryGetComponent(out twentyDamage))
            {
                isInTwentyDamage = true;
                break;
            }
        }

        if (isInTwentyDamage)
        {
            twentyDamage.IncreaseCountdown(twentyDamageCountdownMax);
        }
        else
        {
            skillCaster.SetBuff(countTwentyDamageBuffPrefab, twentyDamageCountdownMax, setBuffTimerMax);
        }
    }
}
