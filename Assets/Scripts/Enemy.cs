using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour, ISkillCaster
{
    [SerializeField] private float enemyMoveSpeed;
    [SerializeField] private float enemyChangeDirTimerMax;
    [SerializeField] private float enemyHeight;
    [SerializeField] private float enemyWidth;
    [SerializeField] private int hpMaxAmount;
    [SerializeField] private int attackCount = 1;
    [SerializeField] private int equippedSkillCountMax = 4;
    [SerializeField] private Transform equippedSkillsTransform;
    [SerializeField] private Transform debuffsTransform;
    [SerializeField] private Vector3 attackPosBias;

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
    private const float EPISILON_DISTANCE = .05f;
    private const float DEFAULT_ATTACK_SPEED = 10f;

    private Rigidbody2D enemyRigidbody;
    private Vector3 moveDir = Vector3.zero;
    private Vector3 battlePosition = Vector3.zero;
    private Vector3 playerBattlePosition = Vector3.zero;
    private float enemyChangeDirTimer;
    private float attackSpeed;
    private float castSkillTimer;
    private float skillCastTime;
    private float debuffMakeEffectTimer;
    private bool isWalking;
    private bool isAttacking;
    private bool isEndingAttack;
    private bool isCastingSkill;
    private bool isDebuffMakingEffect;
    private bool isImprisoned;
    private Orientation orientation;
    private int hpAmount;
    private int attackDamageMin;
    private int attackDamageMax;
    private List<Skill> equippedSkillList = new List<Skill>();


    private void Awake()
    {
        ChangeDir();
        enemyChangeDirTimer = enemyChangeDirTimerMax;

        enemyRigidbody = GetComponent<Rigidbody2D>();

        hpAmount = hpMaxAmount;
        attackSpeed = DEFAULT_ATTACK_SPEED;
    }

    private void Start()
    {
        Player.Instance.OnEnterBattle += Player_OnEnterBattle;
        TurnManager.Instance.OnEnterEnemyTurn += TurnManager_OnEnterEnemyTurn;

        InitializeSkill();
    }

    private void Update()
    {
        AutoChangeDir();

        TryMoveToBattlePosition();

        TryAttack();

        HandleSkillCastTiming();

        TryDebuffMakeEffect();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    public Transform GetDebuffContainerTransform()
    {
        return debuffsTransform;
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
                } else if (TurnManager.Instance.GetTurnState() == TurnManager.Turn.Enemy)
                {
                    CastSkill();
                }
            }
        }
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

    private void CastSkill()
    {
        isCastingSkill = true;
        int castedSkillIndex = UnityEngine.Random.Range(0, equippedSkillList.Count);
        equippedSkillList[castedSkillIndex].CastSkill(this);
    }

    public void SetDebuff(Transform debuffPrefab, int countdownMax, float setDebuffTimerMax)
    {
        if (debuffsTransform.childCount != 0)
        {
            debuffsTransform.GetChild(0).GetComponent<Debuff>().DestroySelf();
        }

        Transform debuffTransform = Instantiate(debuffPrefab, debuffsTransform);
        debuffTransform.GetComponent<Debuff>().Initialize(this, countdownMax, setDebuffTimerMax);
    }

    public ISkillCaster GetOpponent()
    {
        return Player.Instance;
    }

    public void SetCastSkill(float castTime)
    {
        skillCastTime = castTime;
        castSkillTimer = skillCastTime;
    }

    public void EndCastSkill()
    {
        Debug.Log("Enemy End Cast");
        isCastingSkill = false;
        EndTurn();
    }

    public void Heal(int healAmount)
    {
        hpAmount += healAmount;
        hpAmount = Math.Min(hpAmount, hpMaxAmount);

        OnHeal?.Invoke(this, EventArgs.Empty);
    }

    private void InitializeSkill()
    {
        List<Skill> allSkillList = GameLibrary.Instance.getAllSkillList();
        for (int i = 0; i < equippedSkillCountMax; i++)
        {
            Skill randomSkill = allSkillList[UnityEngine.Random.Range(0, allSkillList.Count)];
            Skill skill = Instantiate(randomSkill, equippedSkillsTransform);
            equippedSkillList.Add(skill);
        }
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
            transform.position = Vector3.Lerp(transform.position, playerBattlePosition, Time.deltaTime * attackSpeed);

            if (Vector3.Distance(transform.position, playerBattlePosition) < EPISILON_DISTANCE)
            {
                isAttacking = false;
                isEndingAttack = true;

                int attackDamage = UnityEngine.Random.Range(attackDamageMin, attackDamageMax + 1);
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
                }
            }
        }
    }

    public void EndTurn()
    {
        TurnManager.Instance.EndEnemyTurn();
    }

    private void TurnManager_OnEnterEnemyTurn(object sender, EventArgs e)
    {
        if (debuffsTransform.childCount != 0)
        {
            isDebuffMakingEffect = true;
            debuffMakeEffectTimer = debuffsTransform.GetChild(0).GetComponent<Debuff>().GetDebuffMakeEffectTimerMax();

            foreach (Transform debuffTransform in debuffsTransform)
            {
                if (debuffTransform.TryGetComponent(out Imprison imprison))
                {
                    isImprisoned = true;
                    break;
                }
            }
        } else
        {
            isDebuffMakingEffect = false;
            CastSkill();
        }
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

            playerBattlePosition = RoomManager.Instance.GetCurRoom().GetPlayerBattlePos() + attackPosBias;
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

    public bool IsPlayer()
    {
        return false;
    }
}
