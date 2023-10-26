using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkProperty : MonoBehaviour
{
    [SerializeField] private int singleSetTokenCount = 1;
    [SerializeField] private Transform darkTokenPrefab;
    [SerializeField] private float imprisonHpPercentageThreshold1 = .5f;
    [SerializeField] private float imprisonHpPercentageThreshold2 = .2f;
    [SerializeField] private Transform imprisonDebuffPrefab;
    [SerializeField] private float setDebuffTimerMax = .5f;
    [SerializeField] private int imprisonCountdownMax1 = 1;
    [SerializeField] private int imprisonCountdownMax2 = 2;

    private int makeImprisonedCount;

    private void Start()
    {
        Player.Instance.OnCastSkill += Player_OnCastSkill;
        Player.Instance.OnEndCastSkill += Player_OnEndCastSkill;
    }

    private void Player_OnEndCastSkill(object sender, System.EventArgs e)
    {
        float opponentHpPercentage = (float)Player.Instance.GetOpponent().GetHPAmount() / Player.Instance.GetOpponent().GetHPMaxAmount();
        if (makeImprisonedCount == 0 && opponentHpPercentage < imprisonHpPercentageThreshold1)
        {
            SetImprisonDebuff(imprisonCountdownMax1);
            makeImprisonedCount++;
        }
        if (makeImprisonedCount == 1 && opponentHpPercentage < imprisonHpPercentageThreshold2)
        {
            SetImprisonDebuff(imprisonCountdownMax2);
            makeImprisonedCount++;
        }
    }

    private void SetImprisonDebuff(int countdownMax)
    {
        bool isInImprison = false;
        Imprison imprison = null;
        Transform debuffContainerTransform = Player.Instance.GetOpponent().GetDebuffContainerTransform();
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
            imprison.IncreaseCountdown(countdownMax);
        }
        else
        {
            Player.Instance.GetOpponent().SetDebuff(imprisonDebuffPrefab, countdownMax, setDebuffTimerMax);
        }
    }

    // ºÚ°µ±ê¼Ç
    private void Player_OnCastSkill(object sender, Skill e)
    {
        bool isAttackSkill = false;
        foreach (Skill.SkillType skillType in e.GetSkillTypeList())
        {
            if (skillType == Skill.SkillType.Attack)
            {
                isAttackSkill = true;
                break;
            }
        }

        if (isAttackSkill)
        {
            SetDarkToken();
        }
    }

    private void SetDarkToken()
    {
        bool hasDarkToken = false;
        DarkToken darkToken = null;
        Transform tokenContainerTransform = Player.Instance.GetOpponent().GetTokenContainerTransform();
        foreach (Transform tokenTransform in tokenContainerTransform)
        {
            if (tokenTransform.TryGetComponent(out darkToken))
            {
                hasDarkToken = true;
                break;
            }
        }

        if (hasDarkToken)
        {
            darkToken.IncreaseCount(singleSetTokenCount);
        }
        else
        {
            Player.Instance.GetOpponent().SetToken(darkTokenPrefab, singleSetTokenCount);
        }
    }

    private void OnDestroy()
    {
        
    }
}
