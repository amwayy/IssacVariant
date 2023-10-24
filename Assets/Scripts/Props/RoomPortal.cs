using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPortal : MonoBehaviour
{
    [SerializeField] private RoomManager.RoomType roomType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            RoomManager.Instance.EnterNewRoom(roomType);
        }
    }
}
