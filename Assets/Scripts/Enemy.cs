using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float enemyMoveSpeed;
    [SerializeField] private float enemyChangeDirTimerMax;
    [SerializeField] private float enemyHeight;
    [SerializeField] private float enemyWidth;
    [SerializeField] private float defaultAttackSpeed = 10f;
    [SerializeField] private int hpMaxAmount;
    [SerializeField] private int attackDamage = 40;
    [SerializeField] private int attackCount = 1;

    public event EventHandler<int> OnTakeDamage;

    public enum Orientation
    {
        Front,
        Back,
        Left,
        Right
    }

    private const float ENTER_BATTLE_SPEED = 7f;
    private const float DEFAULT_ATTACK_SPEED = 7f;
    private const float EPISILON_DISTANCE = .05f;

    private Rigidbody2D enemyRigidbody;
    private Vector3 moveDir = Vector3.zero;
    private Vector3 battlePosition = Vector3.zero;
    private Vector3 playerBattlePosition = Vector3.zero;
    private float enemyChangeDirTimer;
    private float attackSpeed;
    private bool isWalking;
    private bool isAttacking;
    private bool isEndingAttack;
    private Orientation orientation;
    private int hpAmount;

    private void Awake()
    {
        ChangeDir();
        enemyChangeDirTimer = enemyChangeDirTimerMax;

        enemyRigidbody = GetComponent<Rigidbody2D>();

        hpAmount = hpMaxAmount;
        attackSpeed = defaultAttackSpeed;
    }

    private void Start()
    {
        Player.Instance.OnEnterBattle += Player_OnEnterBattle;
        TurnManager.Instance.OnEnterEnemyTurn += TurnManager_OnEnterEnemyTurn;
    }

    private void Update()
    {
        AutoChangeDir();

        TryMoveToBattlePosition();

        TryAttack();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void TryAttack()
    {
        if (isAttacking)
        {
            transform.position = Vector3.Lerp(transform.position, playerBattlePosition, Time.deltaTime * attackSpeed);

            //Debug.Log(Vector3.Distance(transform.position, playerBattlePosition));

            if (Vector3.Distance(transform.position, playerBattlePosition) < EPISILON_DISTANCE)
            {
                isAttacking = false;
                isEndingAttack = true;

                Player.Instance.TakeDamage(attackDamage);

                if (Player.Instance.GetHPAmount() == 0)
                {
                    attackCount = 1;
                }
            }
        }

        if (isEndingAttack)
        {
            transform.position = Vector3.Lerp(transform.position, battlePosition, Time.deltaTime * attackSpeed);
            if (Vector3.Distance(transform.position, battlePosition) < EPISILON_DISTANCE)
            {
                isEndingAttack = false;

                attackCount--;
                if (attackCount > 0)
                {
                    isAttacking = true;
                } else
                {
                    TurnManager.Instance.EndEnemyTurn();
                }
            }
        }
    }

    private void TurnManager_OnEnterEnemyTurn(object sender, EventArgs e)
    {
        isAttacking = true;
    }

    public int GetHPAmount()
    {
        return hpAmount;
    }

    public int GetHPMaxAmount()
    {
        return hpMaxAmount;
    }

    public void TakeDamage(int damage)
    {
        hpAmount -= damage;
        hpAmount = Math.Max(0, hpAmount);

        OnTakeDamage?.Invoke(this, damage);

        if (hpAmount == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        DestroySelf();
    }

    private void Player_OnEnterBattle(object sender, Enemy e)
    {
        if (e == this)
        {
            battlePosition = RoomManager.Instance.GetCurRoom().GetEnemyBattlePos();
            orientation = Orientation.Front;

            playerBattlePosition = RoomManager.Instance.GetCurRoom().GetPlayerBattlePos();
        }
        else
        {
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        Player.Instance.OnEnterBattle -= Player_OnEnterBattle;
        TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;

        transform.SetParent(null);
        Destroy(gameObject);
    }

    private void AutoChangeDir()
    {
        enemyChangeDirTimer -= Time.deltaTime;

        if (enemyChangeDirTimer <= 0f)
        {
            ChangeDir();
        }
    }

    private void TryMoveToBattlePosition()
    {
        if (!BattleManager.Instance.IsInBattle()) return;

        if (isAttacking || isEndingAttack) return;

        transform.position = Vector3.Lerp(transform.position, battlePosition, Time.deltaTime * ENTER_BATTLE_SPEED);
    }

    private void UpdateOrientation()
    {
        if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
        {
            if (moveDir.x > 0)
            {
                orientation = Orientation.Right;
            } else
            {
                orientation = Orientation.Left;
            }
        } else
        {
            if (moveDir.y > 0)
            {
                orientation = Orientation.Back;
            } else
            {
                orientation = Orientation.Front;
            }
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    public Orientation GetEnemyOrientation()
    {
        return orientation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ChangeDir();
    }

    private void HandleMovement()
    {
        if (BattleManager.Instance.IsInBattle()) return;

        enemyRigidbody.velocity = moveDir * enemyMoveSpeed;

        if (enemyRigidbody.velocity != Vector2.zero)
        {
            isWalking = true;
            UpdateOrientation();
        }
        else
        {
            isWalking = false;
        }
    }

    private void ChangeDir()
    {
        int dirX = UnityEngine.Random.Range(-10, 10);
        int dirY = UnityEngine.Random.Range(-10, 10);
        moveDir = new Vector3(dirX, dirY, 0);
        moveDir.Normalize();

        enemyChangeDirTimer = enemyChangeDirTimerMax;
    }
}
