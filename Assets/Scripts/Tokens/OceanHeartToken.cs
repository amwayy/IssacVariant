using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanHeartToken : Token
{
    public override void IncreaseCount(int increaseCount)
    {
        base.IncreaseCount(increaseCount);

        if (count == countMax)
        {
            // 只有玩家会有海洋之心
            TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;
        }
    }

    private void TurnManager_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        Transform opponentBuffContainerTransform = Player.Instance.GetOpponent().GetBuffContainerTransform();
        int opponentBuffCount = opponentBuffContainerTransform.childCount;
        for (int i = 0; i < opponentBuffCount; i++)
        {
            opponentBuffContainerTransform.GetChild(0).GetComponent<Buff>().DestroySelf();
        }
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;

        DestroySelf();
    }

    private void OnDestroy()
    {
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
    }
}
