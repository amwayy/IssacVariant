using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpeed : Skill
{
    [SerializeField] private int actionPointModifyAmount = 1;

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
