using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    private void Start()
    {
        Player.Instance.OnQuitBattle += Player_OnQuitBattle;
    }

    private void Player_OnQuitBattle(object sender, System.EventArgs e)
    {
        DestroySelf();
    }

    private void DestroySelf()
    {
        Player.Instance.OnQuitBattle -= Player_OnQuitBattle;
        transform.SetParent(null);
        Destroy(gameObject);
    }
}
