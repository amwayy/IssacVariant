using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            PlayerStatUI.Instance.AddPotion(1);

            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        transform.SetParent(null);
        Destroy(gameObject);
    }
}
