using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private List<Room> regularRoomList;
    [SerializeField] private List<Room> initialRoomList;
    [SerializeField] private Transform chestPrefab;
    [SerializeField] private Transform portalPrefab;

    public static RoomManager Instance { get; private set; }

    public event EventHandler OnEnterNewRoom;

    public enum RoomType
    {
        Regular,
        Initial,
        Shop,
        Boss,
    }

    private Room curRoom;

    private void Awake()
    {
        Instance = this;

        SpawnRoom(RoomType.Initial);
    }

    private void Start()
    {
        Player.Instance.OnQuitBattle += Player_OnQuitBattle;
    }

    private void Player_OnQuitBattle(object sender, EventArgs e)
    {
        Transform chestTransform = Instantiate(chestPrefab, transform);
        chestTransform.position = curRoom.GetChestPos();

        foreach (Vector3 portalPos in curRoom.GetPortalPosList())
        {
            Transform portalTransform = Instantiate(portalPrefab, transform);
            portalTransform.position = portalPos;
        }
    }

    public void EnterNewRoom(RoomType roomType)
    {
        int roomObjectCount = transform.childCount;
        for (int i = 0; i < roomObjectCount; i++)
        {
            Transform child = transform.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }

        SpawnRoom(roomType);

        OnEnterNewRoom?.Invoke(this, EventArgs.Empty);
    }

    private void SpawnRoom(RoomType roomType)
    {
        Room randomRoom = null;
        switch (roomType)
        {
            case RoomType.Regular:
                randomRoom = regularRoomList[UnityEngine.Random.Range(0, regularRoomList.Count)];
                break;
            case RoomType.Initial:
                randomRoom = initialRoomList[UnityEngine.Random.Range(0, initialRoomList.Count)];
                break;
        }
        Instantiate(randomRoom, transform);
        curRoom = randomRoom;
    }

    public Room GetCurRoom()
    {
        return curRoom;
    }
}
