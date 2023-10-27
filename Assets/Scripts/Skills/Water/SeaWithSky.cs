using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaWithSky : Skill
{
    [SerializeField] private int singleSetTokenCount = 1;
    [SerializeField] private Transform weaknessTokenPrefab;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);

        SetWeaknessToken();
    }

    private void SetWeaknessToken()
    {
        bool hasWeaknessToken = false;
        WeaknessToken weaknessToken = null;
        Transform tokenContainerTransform = Player.Instance.GetOpponent().GetTokenContainerTransform();
        foreach (Transform tokenTransform in tokenContainerTransform)
        {
            if (tokenTransform.TryGetComponent(out weaknessToken))
            {
                hasWeaknessToken = true;
                break;
            }
        }

        if (hasWeaknessToken)
        {
            weaknessToken.IncreaseCount(singleSetTokenCount);
        }
        else
        {
            Player.Instance.GetOpponent().SetToken(weaknessTokenPrefab, singleSetTokenCount);
        }
    }
}
