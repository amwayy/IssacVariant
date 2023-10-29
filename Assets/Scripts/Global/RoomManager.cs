using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private List<Room> regularRoomList;
    [SerializeField] private List<Room> initialRoomList;
    [SerializeField] private List<Room> bossRoomList;
    [SerializeField] private Transform chestPrefab;
    [SerializeField] private Transform portalPrefab;
    [SerializeField] private Transform potionPrefab;

    public static RoomManager Instance { get; private set; }

    public event EventHandler OnEnterNewRoom;

    public enum RoomType
    {
        Regular,
        Initial,
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
        SpawnChest();
        SpawnPortal();
        SpawnPotions();
    }

    private void SpawnPotions()
    {
        int spawnCount = UnityEngine.Random.Range(0, GameLibrary.Instance.GetPotionSpawnCountMax() + 1);
        for (int i = 0; i < spawnCount; i++)
        {
            Transform potionTransform = Instantiate(potionPrefab, transform);
            float leftLimit = curRoom.GetLeftLimit();
            float rightLimit = curRoom.GetRightLimit();
            float upLimit = curRoom.GetUpLimit();
            float downLimit = curRoom.GetDownLimit();

            System.Random random = new System.Random();
            float posX = (float)random.NextDouble() * (rightLimit - leftLimit) + leftLimit;
            float posY = (float)random.NextDouble() * (upLimit - downLimit) + downLimit;
            potionTransform.position = new Vector3(posX, posY, 0);
        }
    }

    private void SpawnChest()
    {
        Transform chestTransform = Instantiate(chestPrefab, transform);
        chestTransform.position = curRoom.GetChestPos();
    }

    private void SpawnPortal()
    {
        foreach (Vector3 portalPos in curRoom.GetPortalPosList())
        {
            Transform portalTransform = Instantiate(portalPrefab, transform);
            portalTransform.position = portalPos;

            RoomPortal roomPortal = portalTransform.GetComponent<RoomPortal>();
            if (GameLibrary.Instance.GetLevelCount() == GameLibrary.Instance.GetMidBossLevelIndex()
                || GameLibrary.Instance.GetLevelCount() == GameLibrary.Instance.GetFinalBossLevelIndex())
            {
                roomPortal.SetPortalRoomType(RoomType.Boss);
            } else
            {
                roomPortal.SetPortalRoomType(RoomType.Regular);
            }
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
            case RoomType.Boss:
                randomRoom = bossRoomList[UnityEngine.Random.Range(0, bossRoomList.Count)];
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
