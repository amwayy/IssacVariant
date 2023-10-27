using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlare : Skill
{
    [SerializeField] private int flareCountdownMax = 1;
    [SerializeField] private float setBuffTimerMax = .5f;
    [SerializeField] private Transform flareBuffPrefab;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);


        if (skillCaster.IsPlayer())
        {
            TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;
        }
        else
        {
            TurnManager.Instance.OnEnterEnemyTurn += TurnManager_OnEnterEnemyTurn;
        }
    }

    private void SetFlareBuff()
    {
        bool isInFlare = false;
        FlareBuff flare = null;
        Transform buffContainerTransform = skillCaster.GetBuffContainerTransform();

        // if (buffContainerTransform == null) return;

        foreach (Transform buffTransform in buffContainerTransform)
        {
            if (buffTransform.TryGetComponent(out flare))
            {
                isInFlare = true;
                break;
            }
        }

        if (isInFlare)
        {
            flare.IncreaseCountdown(flareCountdownMax);
        }
        else
        {
            skillCaster.SetBuff(flareBuffPrefab, flareCountdownMax, setBuffTimerMax);
        }
    }

    private void TurnManager_OnEnterEnemyTurn(object sender, System.EventArgs e)
    {
        TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;

        SetFlareBuff();
    }

    private void TurnManager_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;

        SetFlareBuff();
    }

    private void OnDestroy()
    {
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
        TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;
    }
}
