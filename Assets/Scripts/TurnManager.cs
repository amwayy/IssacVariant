using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public event EventHandler OnEnterEnemyTurn;
    public event EventHandler OnEnterPlayerTurn;

    public enum Turn
    {
        Player,
        Enemy,
    }

    private Turn turn = Turn.Player;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Player.Instance.OnTurnEnd += Player_OnTurnEnd;
    }

    public Turn GetTurnState()
    {
        return turn;
    }

    public void EndEnemyTurn()
    {
        turn = Turn.Player;
        OnEnterPlayerTurn?.Invoke(this, EventArgs.Empty);
    }

    private void Player_OnTurnEnd(object sender, System.EventArgs e)
    {
        turn = Turn.Enemy;
        OnEnterEnemyTurn?.Invoke(this, EventArgs.Empty);
    }
}
