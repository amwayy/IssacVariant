using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour, ISkillCaster
{
    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private float playerHeight;
    [SerializeField] private float playerWidth;
    [SerializeField] private int defaultActionPointMaxCount = 3;
    [SerializeField] private int hpMaxAmount = 500;
    [SerializeField] private int defaultEquippedSkillCountMax = 4;
    [SerializeField] private int defaultBackupSkillCountMax = 6;
    [SerializeField] private Vector3 attackPosBias;
    [SerializeField] private Transform debuffContainerTransform;
    [SerializeField] private Transform buffContainerTransform;
    [SerializeField] private int atk = 100;
    [SerializeField] private int def = 100;
    [SerializeField] private Transform equippedSkillsTransform;
    [SerializeField] private Transform backupSkillsTransform;
    [SerializeField] private Transform lootSkillsTransform;

    public static Player Instance { get; private set; }

    public event EventHandler<Enemy> OnEnterBattle;
    public event EventHandler OnQuitBattle;
    public event EventHandler OnCastSkill;
    public event EventHandler OnTurnEnd;
    public event EventHandler<int> OnTakeDamage;
    public event EventHandler OnHeal;
    public event EventHandler<int> OnModifyActionPoint;
    public event EventHandler<int> OnCheckShield;
    public event EventHandler OnEndCastSkill;
    public event EventHandler OnEndLoot;
    public event EventHandler<GameLibrary.Element> OnChangeElement;

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
    private bool isRealDamage;   // 受到的是否为真实伤害，即是否无视护盾
    private int damageTaken;
    private List<int> equippedSkillCoolingCountdownList = new List<int>();
    private List<int> backupSkillCoolingCountdownList = new List<int>();
    private List<Skill> lootSkillList = new List<Skill>();
    private GameLibrary.Element element;

    private void Awake()
    {
        Instance = this;

        playerRigidbody = GetComponent<Rigidbody2D>();

        actionPointMaxCount = defaultActionPointMaxCount;
        availableActionPointCount = actionPointMaxCount;
        hpAmount = hpMaxAmount;
        attackSpeed = DEFAULT_ATTACK_SPEED;
        equippedSkillCountMax = defaultEquippedSkillCountMax;
        backupSkillCountMax = defaultBackupSkillCountMax;

        for (int i = 0; i < equippedSkillCountMax; i++)
        {
            equippedSkillCoolingCountdownList.Add(0);
        }
        for (int i = 0; i < defaultBackupSkillCountMax; i++)
        {
            backupSkillCoolingCountdownList.Add(0);
        }
    }

    private void Start()
    {
        TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;
        RoomManager.Instance.OnEnterNewRoom += RoomManager_OnEnterNewRoom;
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

    public void SetElement(GameLibrary.Element element)
    {
        this.element = element;
        OnChangeElement?.Invoke(this, element);

        InitializeSkill();
    }

    private void RoomManager_OnEnterNewRoom(object sender, EventArgs e)
    {
        transform.position = RoomManager.Instance.GetCurRoom().GetPlayerInitialPos();
    }

    public int GetBackupSkillCountMax()
    {
        return backupSkillCountMax;
    }

    public void EndLoot()
    {
        foreach (Skill skill in lootSkillList)
        {
            skill.transform.SetParent(null);
            Destroy(skill.gameObject);
        }

        lootSkillList.Clear();

        OnEndLoot?.Invoke(this, EventArgs.Empty);
    }

    public void SetBackupSkill(int index, Skill skill)
    {
        if (index >= backUpSkillList.Count) return;

        backUpSkillList[index] = skill;
    }

    public List<Skill> GetLootSkillList()
    {
        return lootSkillList;
    }

    public void SetLootSkillList(List<Skill> lootSkillList)
    {
        this.lootSkillList.Clear();

        foreach (Skill skill in lootSkillList)
        {
            skill.transform.SetParent(lootSkillsTransform);
            this.lootSkillList.Add(skill);
        }
    }

    public int GetEquippedSkillCoolingCountdown(int index)
    {
        return equippedSkillCoolingCountdownList[index];
    }

    public int GetBackupSkillCoolingCountdown(int index)
    {
        return backupSkillCoolingCountdownList[index];
    }

    public void SetEquippedSkillCoolingCountdown(int index, int countdown)
    {
        equippedSkillCoolingCountdownList[index] = countdown;
    }

    public void SetBackupSkillCoolingCountdown(int index, int countdown)
    {
        backupSkillCoolingCountdownList[index] = countdown;

    }

    public void SetDamageTaken(int modifiedDamage)
    {
        damageTaken = modifiedDamage;
    }

    public void ModifyActionPointMax(int modifyAmount)
    {
        actionPointMaxCount += modifyAmount;

        if (modifyAmount > 0)
        {
            availableActionPointCount += modifyAmount;
        }
        if (modifyAmount < 0)
        {
            availableActionPointCount = Math.Min(availableActionPointCount, actionPointMaxCount);
        }

        OnModifyActionPoint?.Invoke(this, modifyAmount);
    }

    public int GetATK()
    {
        return atk;
    }

    public int GetDEF()
    {
        return def;
    }

    public void SetATK(int atk)
    {
        this.atk = atk;
    }
    public void SetDEF(int def)
    {
        this.def = def;
    }

    public void SetAttackModify(int modifyAmount)
    {
        attackModifyAmount = modifyAmount;
    }

    public void ExchangeEquippedLootSkill(int equippedSkillIndex, int lootSkillIndex)
    {
        Skill equippedSkillToExchange = equippedSkillList[equippedSkillIndex];
        Skill lootSkillToExchange = lootSkillList[lootSkillIndex];
        equippedSkillList[equippedSkillIndex] = lootSkillToExchange;
        lootSkillList[lootSkillIndex] = equippedSkillToExchange;

        equippedSkillToExchange.transform.SetParent(lootSkillsTransform);
        equippedSkillToExchange.transform.SetSiblingIndex(lootSkillIndex);
        lootSkillToExchange.transform.SetParent(equippedSkillsTransform);
        lootSkillToExchange.transform.SetSiblingIndex(equippedSkillIndex);
    }

    public void ExchangeBackupLootSkill(int backupSkillIndex, int lootSkillIndex)
    {
        if (backupSkillIndex < backUpSkillList.Count)
        {
            Skill backupSkillToExchange = backUpSkillList[backupSkillIndex];
            Skill lootSkillToExchange = lootSkillList[lootSkillIndex];
            backUpSkillList[backupSkillIndex] = lootSkillToExchange;
            lootSkillList[lootSkillIndex] = backupSkillToExchange;

            backupSkillToExchange.transform.SetParent(lootSkillsTransform);
            backupSkillToExchange.transform.SetSiblingIndex(lootSkillIndex);
            lootSkillToExchange.transform.SetParent(backupSkillsTransform);
            lootSkillToExchange.transform.SetSiblingIndex(backupSkillIndex);
        } else
        {
            Skill lootSkillToExchange = lootSkillList[lootSkillIndex];
            backUpSkillList.Add(lootSkillToExchange);

            lootSkillToExchange.transform.SetParent(backupSkillsTransform);
            lootSkillToExchange.transform.SetSiblingIndex(backupSkillIndex);
        }
    }

    public void ExchangeEquippedBackupSkill(int equippedSkillIndex, int backupSkillIndex)
    {
        Skill equippedSkillToExchange = equippedSkillList[equippedSkillIndex];
        Skill backupSkillToExchange = backUpSkillList[backupSkillIndex];
        equippedSkillList[equippedSkillIndex] = backupSkillToExchange;
        backUpSkillList[backupSkillIndex] = equippedSkillToExchange;

        equippedSkillToExchange.transform.SetParent(backupSkillsTransform);
        equippedSkillToExchange.transform.SetSiblingIndex(backupSkillIndex);
        backupSkillToExchange.transform.SetParent(equippedSkillsTransform);
        backupSkillToExchange.transform.SetSiblingIndex(equippedSkillIndex);
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

    public Buff SetBuff(Transform buffPrefab, int countdownMax, float setBuffTimerMax)
    {
        Transform buffTransform = Instantiate(buffPrefab, buffContainerTransform);
        Buff buff = buffTransform.GetComponent<Buff>();
        buff.Initialize(this, countdownMax, setBuffTimerMax);

        return buff;
    }

    public Debuff SetDebuff(Transform debuffPrefab, int countdownMax, float setDebuffTimerMax, int extraCountdown = 0)
    {
        if (debuffPrefab.TryGetComponent(out Anomaly anomaly))
        {
            foreach (Transform debuffChildTransform in debuffContainerTransform)
            {
                if (debuffChildTransform.TryGetComponent(out Anomaly oldAnomaly))
                {
                    oldAnomaly.DestroySelf();
                    break;
                }
            }
        }

        Transform debuffTransform = Instantiate(debuffPrefab, debuffContainerTransform);
        Debuff debuff = debuffTransform.GetComponent<Debuff>();
        debuff.Initialize(this, countdownMax, setDebuffTimerMax, extraCountdown);

        return debuff; 
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
        List<Skill> skillList = new List<Skill>();
        switch(element)
        {
            case GameLibrary.Element.Grass:
                skillList = GameLibrary.Instance.GetElementSkillList(GameLibrary.Element.Grass);
                break;
            case GameLibrary.Element.Fire:
                skillList = GameLibrary.Instance.GetElementSkillList(GameLibrary.Element.Fire);
                break;
            case GameLibrary.Element.Water:
                skillList = GameLibrary.Instance.GetElementSkillList(GameLibrary.Element.Water);
                break;
            case GameLibrary.Element.Light:
                skillList = GameLibrary.Instance.GetElementSkillList(GameLibrary.Element.Light);
                break;
            case GameLibrary.Element.Dark:
                skillList = GameLibrary.Instance.GetElementSkillList(GameLibrary.Element.Dark);
                break;
        }
        for (int i = 0; i < equippedSkillCountMax; i++)
        {
            Skill equippedSkill = Instantiate(skillList[skillList.Count - 1 - i], equippedSkillsTransform);
            equippedSkillList.Add(equippedSkill);
        }

        // 把所有技能中的前几个放到背包里
        List<Skill> allSkillList = GameLibrary.Instance.GetAllSkillList();
        int backupSkillCount = Math.Min(allSkillList.Count - equippedSkillCountMax, backupSkillCountMax);
        // int backupSkillCount = 6;
        for (int i = 0; i < backupSkillCount; i++)
        {
            Skill backupSkill = Instantiate(allSkillList[i], backupSkillsTransform);
            backUpSkillList.Add(backupSkill);
        }
    }

    public void EndTurn()
    {
        OnTurnEnd?.Invoke(this, EventArgs.Empty);
    }

    private void TurnManager_OnEnterPlayerTurn(object sender, EventArgs e)
    {
        ModifyActionPointMax(defaultActionPointMaxCount - actionPointMaxCount);
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
                if (debuffTransform.TryGetComponent(out Drown drown) && drown.GetCountdown() > 1)
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

    public void TakeDamage(int damage, bool isRealDamageTaken = false)
    {
        damageTaken = damage;

        // Check shield
        if (!isRealDamageTaken)
        {
            foreach (Transform buffTransform in buffContainerTransform)
            {
                if (buffTransform.TryGetComponent(out Shield shield))
                {
                    OnCheckShield?.Invoke(this, damageTaken);
                    break;
                }
            }
        }

        hpAmount -= damageTaken;
        hpAmount = Math.Max(0, hpAmount);

        OnTakeDamage?.Invoke(this, damageTaken);

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

        isRealDamage = false;

        OnEndCastSkill?.Invoke(this, EventArgs.Empty);

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

    public void SetAttack(int damageMin, int damageMax, float playerAttackSpeed = DEFAULT_ATTACK_SPEED, int attackCount = 1, bool isRealDamage = false)
    {
        isAttacking = true;
        attackDamageMin = (int)((float)damageMin * atk / GetOpponent().GetDEF());
        attackDamageMax = (int)((float)damageMax * atk / GetOpponent().GetDEF());
        attackSpeed = playerAttackSpeed;
        this.attackCount = attackCount;
        attackModifyAmount = 0;
        this.isRealDamage = isRealDamage;
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
                battlingEnemy.TakeDamage(attackDamage, isRealDamage);

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
