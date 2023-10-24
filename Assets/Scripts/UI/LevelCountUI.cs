using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelCountUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelCountText;

    private int levelCount;

    private void Start()
    {
        Player.Instance.OnQuitBattle += Player_OnQuitBattle;
    }

    private void Player_OnQuitBattle(object sender, System.EventArgs e)
    {
        levelCount++;
        levelCountText.text = levelCount.ToString();
    }
}
