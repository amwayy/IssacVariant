using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPortal : MonoBehaviour
{
    private RoomManager.RoomType roomType;

    public void SetPortalRoomType(RoomManager.RoomType roomType)
    {
        this.roomType = roomType;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            RoomManager.Instance.EnterNewRoom(roomType);
        }
    }
}
