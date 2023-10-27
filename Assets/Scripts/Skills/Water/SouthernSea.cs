using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SouthernSea : Skill
{
    [SerializeField] private int doubleDamageCountdownMax = 1;
    [SerializeField] private float setBuffTimerMax = 0f;
    [SerializeField] private Transform doubleDamagePrefab;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        if (skillCaster.IsPlayer())
        {
            TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;
        } else
        {
            TurnManager.Instance.OnEnterEnemyTurn += TurnManager_OnEnterEnemyTurn;
        }
    }

    private void TurnManager_OnEnterEnemyTurn(object sender, System.EventArgs e)
    {
        TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;

        SetDoubleDamageBuff();
    }

    private void TurnManager_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;

        SetDoubleDamageBuff();
    }

    private void SetDoubleDamageBuff()
    {
        bool isInDoubleDamage = false;
        TurnDoubleDamage doubleDamage = null;
        Transform buffContainerTransform = skillCaster.GetBuffContainerTransform();

        if (buffContainerTransform == null) return;

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
            skillCaster.SetBuff(doubleDamagePrefab, doubleDamageCountdownMax, setBuffTimerMax);
        }
    }

    private void OnDestroy()
    {
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
        TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;
    }
}
