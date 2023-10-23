using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchBlack : Skill
{
    [SerializeField] private int thisActionPointExpense = 1;
    [SerializeField] private int atkUpCountdownMax = 1;
    [SerializeField] private float thisCastTime = 1f;
    [SerializeField] private float setBuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private Transform atkUpDebuffPrefab;
    [SerializeField] private float atkUpPercentage = .2f;

    private void Awake()
    {
        skillName = "Pitch Black";   // 漆黑
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

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
        SetAtkUpBuff();
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
        TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;
    }

    private void TurnManager_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        SetAtkUpBuff();
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
        TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;
    }

    private void SetAtkUpBuff()
    {
        bool isInAtkUp = false;
        AtkUp atkUp = null;
        Transform buffContainerTransform = skillCaster.GetBuffContainerTransform();
        foreach (Transform buffTransform in buffContainerTransform)
        {
            if (buffTransform.TryGetComponent(out atkUp))
            {
                isInAtkUp = true;
                break;
            }
        }

        if (isInAtkUp)
        {
            atkUp.IncreaseCountdown(atkUpCountdownMax);
        }
        else
        {
            Buff buff = skillCaster.SetBuff(atkUpDebuffPrefab, atkUpCountdownMax, setBuffTimerMax);
            buff.GetComponent<AtkUp>().SetAtkUpPercentage(atkUpPercentage);
        }
    }
}
