using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private List<Room> regularRoomList;

    public static RoomManager Instance { get; private set; }

    private Room curRoom;

    private void Awake()
    {
        Instance = this;

        InitializeRoom();
    }

    private void InitializeRoom()
    {
        Room randomRoom = regularRoomList[Random.Range(0, regularRoomList.Count)];
        Instantiate(randomRoom, transform);
        curRoom = randomRoom;
    }

    public Room GetCurRoom()
    {
        return curRoom;
    }
}
