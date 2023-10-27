using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private Transform playerInitialPos;
    [SerializeField] private Transform playerBattlePosTransform;
    [SerializeField] private Transform enemyBattlePosTransform;
    [SerializeField] private Transform chestPosTransform;
    [SerializeField] private List<Transform> portalPosTransformList;
    [SerializeField] private RoomManager.RoomType roomType;
    [SerializeField] private float leftLimit;
    [SerializeField] private float rightLimit;
    [SerializeField] private float upLimit;
    [SerializeField] private float downLimit;

    public float GetLeftLimit()
    {
        return leftLimit;
    }

    public float GetRightLimit()
    {
        return rightLimit;
    }

    public float GetUpLimit()
    {
        return upLimit;
    }

    public float GetDownLimit()
    {
        return downLimit;
    }

    public RoomManager.RoomType GetRoomType()
    {
        return roomType;
    }

    public List<Vector3> GetPortalPosList()
    {
        List<Vector3> portalPosList = new List<Vector3>();
        foreach (Transform portalPosTransform in portalPosTransformList)
        {
            portalPosList.Add(portalPosTransform.position);
        }
        return portalPosList;
    }

    public Vector3 GetPlayerInitialPos()
    {
        return playerInitialPos.position;
    }

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
