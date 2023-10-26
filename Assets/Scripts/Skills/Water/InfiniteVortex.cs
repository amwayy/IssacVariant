using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteVortex : Skill
{
    [SerializeField] private int baseDamage = 100;
    [SerializeField] private int damageDelta = 5;
    [SerializeField] private int damageCountdownMax = 2;
    [SerializeField] private int imprisonCountdownMax = 1;
    [SerializeField] private float drownProbability = .5f;
    [SerializeField] private float setDebuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private float backDamagePercentage = .3f;   // 反伤百分比
    [SerializeField] private Transform drownDebuffPrefab;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        int pinnedBaseDamage = Random.Range(baseDamage - damageDelta, baseDamage + damageDelta + 1);
        skillCaster.SetAttack(pinnedBaseDamage, pinnedBaseDamage);

        int finalDamage = (int)((float)pinnedBaseDamage * skillCaster.GetATK() / skillCaster.GetOpponent().GetDEF());
        skillCaster.TakeDamage((int)(finalDamage * backDamagePercentage));

        System.Random random = new System.Random();
        float randomNum = (float)random.NextDouble();
        if (randomNum <= drownProbability)
        {
            SetDrownDebuff();
        }
    }

    private void SetDrownDebuff()
    {
        bool isInDrown = false;
        Bleed drown = null;
        Transform debuffContainerTransform = skillCaster.GetOpponent().GetDebuffContainerTransform();
        foreach (Transform debuffTransform in debuffContainerTransform)
        {
            if (debuffTransform.TryGetComponent(out drown))
            {
                isInDrown = true;
                break;
            }
        }

        if (isInDrown)
        {
            drown.IncreaseCountdown(damageCountdownMax);
        }
        else
        {
            skillCaster.GetOpponent().SetDebuff(drownDebuffPrefab, damageCountdownMax, setDebuffTimerMax, imprisonCountdownMax);
        }
    }
}
