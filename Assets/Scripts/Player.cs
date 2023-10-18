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
    [SerializeField] private int defaultBackupSkillCountMax = 6;
    [SerializeField] private Vector3 attackPosBias;
    [SerializeField] private Transform debuffContainerTransform;
    [SerializeField] private Transform buffContainerTransform;

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

    private const float ENTER_BATTLE_SPEED = 15f;
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
    private bool isDebuffMakingEffect;
    private bool isImprisoned;
    private Orientation orientation = Orientation.Front;
    private float attackSpeed;
    private float castSkillTimer;
    private float skillCastTime;
    private float debuffMakeEffectTimer;
    private int attackCount = 0;
    private int attackDamageMin;
    private int attackDamageMax;
    private int lastAttackDamage;
    private int actionPointMaxCount;
    private int availableActionPointCount;
    private int hpAmount;
    private int equippedSkillCountMax;
    private int backupSkillCountMax;
    private int attackModifyAmount;
    private Enemy battlingEnemy;
    private List<Skill> equippedSkillList = new List<Skill>();
    private List<Skill> backUpSkillList = new List<Skill>();

    private void Awake()
    {
        Instance = this;

        playerRigidbody = GetComponent<Rigidbody2D>();

        actionPointMaxCount = initialActionPointMaxCount;
        availableActionPointCount = actionPointMaxCount;
        hpAmount = hpMaxAmount;
        attackSpeed = DEFAULT_ATTACK_SPEED;
        equippedSkillCountMax = defaultEquippedSkillCountMax;
        backupSkillCountMax = defaultBackupSkillCountMax;
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

        TryDebuffMakeEffect();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    public void SetAttackModify(int modifyAmount)
    {
        attackModifyAmount = modifyAmount;
    }

    public void ExchangeSkill(int equippedSkillIndex, int backupSkillIndex)
    {
        Skill equippedSkillToExchange = equippedSkillList[equippedSkillIndex];
        Skill backupSkillToExchange = backUpSkillList[backupSkillIndex];
        equippedSkillList[equippedSkillIndex] = backupSkillToExchange;
        backUpSkillList[backupSkillIndex] = equippedSkillToExchange;
    }

    public List<Skill> GetBackupSkillList()
    {
        return backUpSkillList;
    }

    public bool IsDebuffMakingEffect()
    {
        return isDebuffMakingEffect;
    }

    private void TryDebuffMakeEffect()
    {
        if (isDebuffMakingEffect)
        {
            debuffMakeEffectTimer -= Time.deltaTime;

            if (debuffMakeEffectTimer <= 0f)
            {
                isDebuffMakingEffect = false;

                if (isImprisoned)
                {
                    EndTurn();
                    isImprisoned = false;
                }
            }
        }
    }
    public Transform GetBuffContainerTransform()
    {
        return buffContainerTransform;
    }

    public Transform GetDebuffContainerTransform()
    {
        return debuffContainerTransform;
    }
    public void SetBuff(Transform buffPrefab, int countdownMax, float setBuffTimerMax)
    {
        Transform buffTransform = Instantiate(buffPrefab, buffContainerTransform);
        buffTransform.GetComponent<Buff>().Initialize(this, countdownMax, setBuffTimerMax);
    }

    public void SetDebuff(Transform debuffPrefab, int countdownMax, float setDebuffTimerMax)
    {
        if (debuffContainerTransform.childCount != 0)
        {
            debuffContainerTransform.GetChild(0).GetComponent<Debuff>().DestroySelf();
        }

        Transform debuffTransform = Instantiate(debuffPrefab, debuffContainerTransform);
        debuffTransform.GetComponent<Debuff>().Initialize(this, countdownMax, setDebuffTimerMax);
    }

    public ISkillCaster GetOpponent()
    {
        return battlingEnemy;
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
        // 初期开发设定

        // player携带全部技能列表的后4个（方便测试最新技能）
        List<Skill> allSkillList = GameLibrary.Instance.getAllSkillList();
        for (int i = 0; i < equippedSkillCountMax; i++)
        {
            equippedSkillList.Add(allSkillList[allSkillList.Count - 1 - i]);
        }

        // 剩下的技能加入背包
        for (int i = 0; i < allSkillList.Count - equippedSkillCountMax; i++)
        {
            backUpSkillList.Add(allSkillList[i]);
        }
    }

    public void EndTurn()
    {
        OnTurnEnd?.Invoke(this, EventArgs.Empty);
    }

    private void TurnManager_OnEnterPlayerTurn(object sender, EventArgs e)
    {
        availableActionPointCount = actionPointMaxCount;

        if (debuffContainerTransform.childCount != 0)
        {
            isDebuffMakingEffect = true;
            debuffMakeEffectTimer = debuffContainerTransform.GetChild(0).GetComponent<Debuff>().GetDebuffMakeEffectTimerMax();

            foreach (Transform debuffTransform in debuffContainerTransform)
            {
                if (debuffTransform.TryGetComponent(out Imprison imprison))
                {
                    isImprisoned = true;
                    break;
                }
            }
        }
        else
        {
            isDebuffMakingEffect = false;
        }
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
        // Check shield
        foreach (Transform buffTransform in buffContainerTransform)
        {
            if (buffTransform.TryGetComponent(out Shield shield))
            {
                float shieldModifier = shield.GetShieldModifier();
                damage = (int)(damage * shieldModifier);
                break;
            }
        }

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
        attackModifyAmount = 0;
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
                lastAttackDamage = attackDamage;
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

                    if (attackModifyAmount > 0)
                    {
                        attackDamageMin = lastAttackDamage + attackModifyAmount;
                        attackDamageMax = lastAttackDamage + attackModifyAmount;
                    }
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
