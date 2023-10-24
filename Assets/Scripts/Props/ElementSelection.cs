using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementSelection : MonoBehaviour
{
    [SerializeField] private GameLibrary.Element element;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            Player.Instance.SetElement(element);

            RoomManager.Instance.EnterNewRoom(RoomManager.RoomType.Regular);
        }
    }
}
