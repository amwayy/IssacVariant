using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private Transform playerBattlePosTransform;
    [SerializeField] private Transform enemyBattlePosTransform;
    [SerializeField] private Transform chestPosTransform;

    public Vector3 GetChestPos()
    {
        return chestPosTransform.position;
    }

    public Vector3 GetPlayerBattlePos()
    {
        return playerBattlePosTransform.position;
    }

    public Vector3 GetEnemyBattlePos()
    {
        return enemyBattlePosTransform.position;
    }
}
