using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpLeaf : Skill
{
    [SerializeField] private int baseDamage = 75;
    [SerializeField] private int damageDelta = 5;
    [SerializeField] private int bleedCountdownMax = 4;
    [SerializeField] private float bleedProbability = .2f;
    [SerializeField] private float setDebuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private Transform bleedDebuffPrefab;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);

        System.Random random = new System.Random();
        float randomNum = (float) random.NextDouble();
        if (randomNum <= bleedProbability)
        {
            SetBleedDebuff();
        }
    }

    private void SetBleedDebuff()
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
