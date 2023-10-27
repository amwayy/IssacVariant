using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiningNoah : Skill
{
    [SerializeField] private float blindProbability = .3f;
    [SerializeField] private int blindCountdownMax = 1;
    [SerializeField] private Transform blindDebuffPrefab;
    [SerializeField] private float setDebuffTimerMax = .5f;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);

        // 致盲
        System.Random random = new System.Random();
        float randomNum = (float)random.NextDouble();
        if (randomNum < blindProbability)
        {
            SetBlindDebuff();
        }

        // 清除敌方增益
        ClearOpponentBuff();
    }

    private void ClearOpponentBuff()
    {
        Transform opponentBuffsTransform = skillCaster.GetOpponent().GetBuffContainerTransform();
        int opponentBuffCount = opponentBuffsTransform.childCount;
        for (int i = 0; i < opponentBuffCount; i++)
        {
            opponentBuffsTransform.GetChild(0).GetComponent<Buff>().DestroySelf();
        }
    }

    private void SetBlindDebuff()
    {
        bool isBlind = false;
        Blind blindDebuff = null;
        Transform debuffContainerTransform = Player.Instance.GetOpponent().GetDebuffContainerTransform();
        foreach (Transform debuffTransform in debuffContainerTransform)
        {
            if (debuffTransform.TryGetComponent(out blindDebuff))
            {
                isBlind = true;
                break;
            }
        }

        if (isBlind)
        {
            blindDebuff.IncreaseCountdown(blindCountdownMax);
        }
        else
        {
            Player.Instance.GetOpponent().SetDebuff(blindDebuffPrefab, blindCountdownMax, setDebuffTimerMax);
        }
    }
}
