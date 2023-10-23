using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpeed : Skill
{
    [SerializeField] private int thisActionPointExpense = 1;
    [SerializeField] private float thisCastTime = 1f;
    [SerializeField] private int actionPointModifyAmount = 1;

    private void Awake()
    {
        skillName = "Light Speed";
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;
    }

    private void TurnManager_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        Player.Instance.ModifyActionPointMax(actionPointModifyAmount);

        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
    }

    private void OnDestroy()
    {
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
    }
}
