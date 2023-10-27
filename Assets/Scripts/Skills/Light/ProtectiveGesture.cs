using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectiveGesture : Skill
{
    [SerializeField] private int defUpCountdownMax = 2;
    [SerializeField] private float defUpPercentage = .2f;
    [SerializeField] private Transform defUpBuffPrefab;
    [SerializeField] private float setBuffTimerMax = .5f;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        if (skillCaster.IsPlayer())
        {
            TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;
        }
        else
        {
            TurnManager.Instance.OnEnterEnemyTurn += TurnManager_OnEnterEnemyTurn;
        }
    }

    private void TurnManager_OnEnterEnemyTurn(object sender, System.EventArgs e)
    {
        TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;

        SetDefUpBuff();
    }

    private void TurnManager_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;

        SetDefUpBuff();
    }

    private void SetDefUpBuff()
    {
        bool isInDefUp = false;
        DefUp defUp = null;
        Transform buffContainerTransform = skillCaster.GetBuffContainerTransform();

        if (buffContainerTransform == null) return;

        foreach (Transform buffTransform in buffContainerTransform)
        {
            if (buffTransform.TryGetComponent(out defUp))
            {
                isInDefUp = true;
                break;
            }
        }

        if (isInDefUp)
        {
            defUp.IncreaseCountdown(defUpCountdownMax);
        }
        else
        {
            Buff buff = skillCaster.SetBuff(defUpBuffPrefab, defUpCountdownMax, setBuffTimerMax);
            buff.GetComponent<DefUp>().SetDefUpPercentage(defUpPercentage);
        }
    }

    private void OnDestroy()
    {
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
        TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;
    }
}
