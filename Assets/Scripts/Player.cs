using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour, ISkillCaster
{
    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private float playerHeight;
    [SerializeField] private float playerWidth;
    [SerializeField] private int initialActionPointMaxCount = 3;
    [SerializeField] private int hpMaxAmount = 500;
    [SerializeField] private int defaultEquippedSkillCountMax = 4;
    [SerializeField] private Vector3 attackPosBias;
    [SerializeField] private Transform equippedSkillsTransform;

    public static Player Instance { get; private set; }

    public event EventHandler<Enemy> OnEnterBattle;
    public event EventHandler OnQuitBattle;
    public event EventHandler OnCastSkill;
    public event EventHandler OnTurnEnd;
    public event EventHandler<int> OnTakeDamage;
    public event EventHandler OnHeal;

    public enum Orientation
    {
        Front,
        Back,
        Left,
        Right
    }

    private const float ENTER_BATTLE_SPEED = 7f;
    private const float DEFAULT_ATTACK_SPEED = 15f;
    private const float EPISILON_DISTANCE = .05f;

    private Rigidbody2D playerRigidbody;
    private Vector3 moveDir = Vector3.zero;
    private Vector3 battlePosition = Vector3.zero;
    private Vector3 enemyBattlePosition = Vector3.zero;
    private bool isWalking;
    private bool isAttacking;
    private bool isEndingAttack;
    private bool isCastingSkill;
    private Orientation orientation = Orientation.Front;
    private float attackSpeed;
    private float castSkillTimer;
    private float skillCastTime;
    private int attackCount = 0;
    private int attackDamageMin;
    private int attackDamageMax;
    private int actionPointMaxCount;
    private int availableActionPointCount;
    private int hpAmount;
    private int equippedSkillCountMax;
    private Enemy battlingEnemy;
    private List<Skill> equippedSkillList = new List<Skill>();

    private void Awake()
    {
        Instance = this;

        playerRigidbody = GetComponent<Rigidbody2D>();

        actionPointMaxCount = initialActionPointMaxCount;
        availableActionPointCount = actionPointMaxCount;
        hpAmount = hpMaxAmount;
        attackSpeed = DEFAULT_ATTACK_SPEED;
        equippedSkillCountMax = defaultEquippedSkillCountMax;
    }

    private void Start()
    {
        TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;

        InitializeSkill();
    }

    private void Update()
    {
        UpdateMoveDir();

        TryMoveToBattlePosition();

        TryAttack();

        HandleSkillCastTiming();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleSkillCastTiming()
    {
        if (isCastingSkill)
        {
            castSkillTimer -= Time.deltaTime;
            if (castSkillTimer <= 0f)
            {
                EndCastSkill();
            }
        }
    }

    public void Heal(int healAmount)
    {
        hpAmount += healAmount;
        hpAmount = Math.Min(hpAmount, hpMaxAmount);

        OnHeal?.Invoke(this, EventArgs.Empty);
    }

    public List<Skill> GetEquippedSkillList()
    {
        return equippedSkillList;
    }

    private void InitializeSkill()
    {
        List<Skill> allSkillList = SkillManager.Instance.getAllSkillList();
        for (int i = 0; i < equippedSkillCountMax; i++)
        {
            Skill randomSkill = allSkillList[UnityEngine.Random.Range(0, allSkillList.Count)];
            Skill skill = Instantiate(randomSkill, equippedSkillsTransform);
            equippedSkillList.Add(skill);
        }
    }

    public void EndTurn()
    {
        OnTurnEnd?.Invoke(this, EventArgs.Empty);
    }

    private void TurnManager_OnEnterPlayerTurn(object sender, EventArgs e)
    {
        availableActionPointCount = actionPointMaxCount;
    }

    public int GetHPMaxAmount()
    {
        return hpMaxAmount;
    }

    public int GetHPAmount()
    {
        return hpAmount;
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
        Debug.Log("Game Over");
    }

    public int GetAvailableActionPointCount()
    {
        return availableActionPointCount;
    }

    public void UseActionPoints(int actionPointExpense)
    {
        availableActionPointCount -= actionPointExpense;
    }

    public void SetCastSkill(float castTime)
    {
        isCastingSkill = true;
        skillCastTime = castTime;
        castSkillTimer = skillCastTime;

        OnCastSkill?.Invoke(this, EventArgs.Empty);
    }

    public void EndCastSkill()
    {
        Debug.Log("Player End Cast");

        isCastingSkill = false;

        if (availableActionPointCount == 0)
        {
            OnTurnEnd?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsCastingSkill()
    {
        return isCastingSkill;
    }

    public Enemy GetBattlingEnemy()
    {
        return battlingEnemy;
    }

    public void SetAttack(int damageMin, int damageMax, float playerAttackSpeed = DEFAULT_ATTACK_SPEED, int attackCount = 1)
    {
        isAttacking = true;
        attackDamageMin = damageMin;
        attackDamageMax = damageMax;
        attackSpeed = playerAttackSpeed;
        this.attackCount = attackCount;
    }

    private void TryAttack()
    {
        if (isAttacking)
        {
            transform.position = Vector3.Lerp(transform.position, enemyBattlePosition, Time.deltaTime * attackSpeed);

            if (Vector3.Distance(transform.position, enemyBattlePosition) < EPISILON_DISTANCE)
            {
                isAttacking = false;
                isEndingAttack = true;

                int attackDamage = UnityEngine.Random.Range(attackDamageMin, attackDamageMax + 1);
                battlingEnemy.TakeDamage(attackDamage);

                if (battlingEnemy.GetHPAmount() == 0)
                {
                    attackCount = 1;
                    OnQuitBattle?.Invoke(this, EventArgs.Empty);
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
                }
            }
        }
    }

    private void TryMoveToBattlePosition()
    {
        if (!BattleManager.Instance.IsInBattle()) return;

        if (isAttacking || isEndingAttack) return;

        transform.position = Vector3.Lerp(transform.position, battlePosition, Time.deltaTime * ENTER_BATTLE_SPEED);
    }

    public Orientation GetPlayerOrientation()
    {
        return orientation;
    }

    private void UpdateOrientation()
    {
        if (moveDir.y < 0)
        {
            orientation = Orientation.Front;
        } else if (moveDir.y > 0)
        {
            orientation = Orientation.Back;
        } else if (moveDir.x < 0)
        {
            orientation = Orientation.Left;
        } else
        {
            orientation = Orientation.Right;
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleMovement()
    {
        if (BattleManager.Instance.IsInBattle()) return;

        playerRigidbody.velocity = moveDir * playerMoveSpeed;

        if (playerRigidbody.velocity != Vector2.zero)
        {
            isWalking = true;
            UpdateOrientation();
        } else
        {
            isWalking = false;
        }
    }

    private void UpdateMoveDir()
    {
        moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDir.y = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x = -1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir.y = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir.x = 1;
        }
        moveDir.Normalize();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Enemy enemy) && !BattleManager.Instance.IsInBattle())
        {
            OnEnterBattle?.Invoke(this, enemy);
            battlePosition = RoomManager.Instance.GetCurRoom().GetPlayerBattlePos();
            enemyBattlePosition = RoomManager.Instance.GetCurRoom().GetEnemyBattlePos() + attackPosBias;
            orientation = Orientation.Back;

            battlingEnemy = enemy;
        }
    }

    public bool IsPlayer()
    {
        return true;
    }
}
