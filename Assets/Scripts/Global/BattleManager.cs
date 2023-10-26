using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private Transform battleUIPrefab;

    public static BattleManager Instance { get; private set; }

    private bool isInBattle = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Player.Instance.OnEnterBattle += Player_OnEnterBattle;
        Player.Instance.OnQuitBattle += Player_OnQuitBattle;
    }

    private void Player_OnQuitBattle(object sender, System.EventArgs e)
    {
        isInBattle = false;
    }

    private void Player_OnEnterBattle(object sender, Enemy e)
    {
        isInBattle = true;

        Instantiate(battleUIPrefab, canvasTransform);
    }

    public bool IsInBattle()
    {
        return isInBattle;
    }
}
