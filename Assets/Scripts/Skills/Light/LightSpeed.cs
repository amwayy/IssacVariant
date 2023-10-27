using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpeed : Skill
{
    [SerializeField] private int actionPointModifyAmount = 1;
    [SerializeField] private int enemyBaseDamage = 60;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        if (skillCaster.IsPlayer())
        {
            TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;
        } else
        {
            skillCaster.SetAttack(enemyBaseDamage - damageDelta, enemyBaseDamage + damageDelta);
        }
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
