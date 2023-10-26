using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassProperty : MonoBehaviour
{
    [SerializeField] private float damageModifyPercentage = .9f;
    [SerializeField] private float healAmountModifyPercentage = 1.1f;
    [SerializeField] private Transform seedTokenPrefab;
    [SerializeField] private int singleSetTokenCount = 1;

    private void Start()
    {
        Player.Instance.OnAttacked += Player_OnAttacked;
        Player.Instance.OnStartHeal += Player_OnStartHeal;
        Player.Instance.OnCastSkill += Player_OnCastSkill;
    }

    // ºı…À
    private void Player_OnAttacked(object sender, ISkillCaster.OnAttackedEventArgs e)
    {
        if (!e.isRealDamage)
        {
            Player.Instance.ModifyDamageTaken(damageModifyPercentage - 1);
        }
    }

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
            SetSeedToken();
        }
    }

    private void SetSeedToken()
    {
        bool hasSeedToken = false;
        SeedToken seedToken = null;
        Transform tokenContainerTransform = Player.Instance.GetOpponent().GetTokenContainerTransform();
        foreach (Transform tokenTransform in tokenContainerTransform)
        {
            if (tokenTransform.TryGetComponent(out seedToken))
            {
                hasSeedToken = true;
                break;
            }
        }

        if (hasSeedToken)
        {
            seedToken.IncreaseCount(singleSetTokenCount);
        }
        else
        {
            Player.Instance.GetOpponent().SetToken(seedTokenPrefab, singleSetTokenCount);
        }
    }

    private void Player_OnStartHeal(object sender, int e)
    {
        Player.Instance.SetHealAmount((int)(e * healAmountModifyPercentage));
    }

    private void OnDestroy()
    {
        Player.Instance.OnAttacked -= Player_OnAttacked;
        Player.Instance.OnStartHeal -= Player_OnStartHeal;
        Player.Instance.OnCastSkill -= Player_OnCastSkill;
    }
}
