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
    [SerializeField] private int defaultEquippedSkillCountMax = 4;
    [SerializeField] private int defaultBackupSkillCountMax = 6;
    [SerializeField] private Vector3 attackPosBias;
    [SerializeField] private Transform debuffContainerTransform;
    [SerializeField] private Transform buffContainerTransform;
    [SerializeField] private int baseAtk = 100;
    [SerializeField] private int baseDef = 100;
    [SerializeField] private int baseHpMax = 100;
    [SerializeField] private Transform equippedSkillsTransform;
    [SerializeField] private Transform backupSkillsTransform;
    [SerializeField] private Transform lootSkillsTransform;
    [SerializeField] private Transform statContainerTransform;
    [SerializeField] private Transform tokenContainerTransform;

    public static Player Instance { get; private set; }

    public event EventHandler<Enemy> OnEnterBattle;
    public event EventHandler OnQuitBattle;
    public event EventHandler<Skill> OnCastSkill;
    public event EventHandler OnTurnEnd;
    public event EventHandler<int> OnTakeDamage;
    public event EventHandler<int> OnStartHeal;
    public event EventHandler OnEndHeal;
    public event EventHandler<int> OnModifyActionPoint;
    public event EventHandler OnEndCastSkill;
    public event EventHandler OnEndLoot;
    public event EventHandler<GameLibrary.Element> OnChangeElement;
    public event EventHandler OnAttackReady;
    public event EventHandler<ISkillCaster.OnAttackedEventArgs> OnAttacked;   // 被攻击、结算伤害之前触发，用于护盾之外的所受伤害调整
    public event EventHandler OnHPMaxModified;

    public enum Orientation
    {
        Front,
        Back,
        Left,
        Right
    }

    private const float ENTER_BATTLE_SPEED = 15f;
    private const float DEFAULT_ATTACK_SPEED = 15f;
    private const float EPISILON_DISTANCE = .1f;

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
    private int hpAmount = -1;
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
    private int atkBeforeBattle;
    private int defBeforeBattle;
    private int healAmount;
    private float statIconSize = .7f;
    private int extraHp;
    private int extraHpMax;
    private Skill lastCastSkill;
    private int hpMaxAmount;
    private int atk;
    private int def;

    private void Awake()
    {
        Instance = this;

        playerRigidbody = GetComponent<Rigidbody2D>();

        actionPointMaxCount = defaultActionPointMaxCount;
        availableActionPointCount = actionPointMaxCount;
        attackSpeed = DEFAULT_ATTACK_SPEED;
        equippedSkillCountMax = defaultEquippedSkillCountMax;
        backupSkillCountMax = defaultBackupSkillCountMax;

        buffContainerTransform.gameObject.SetActive(false);
        debuffContainerTransform.gameObject.SetActive(false);
        tokenContainerTransform.gameObject.SetActive(false);

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

    private void UpdateAttribute()
    {
        atk = baseAtk;
        def = baseDef;
        hpMaxAmount = baseHpMax;

        foreach (Skill equippedSkill in equippedSkillList)
        {
            if (equippedSkill.GetElement() == element)
            {
                atk += equippedSkill.GetAttack();
                def += equippedSkill.GetDefense();
                hpMaxAmount += equippedSkill.GetHealth();
            }
        }

        foreach (Skill backupSkill in backUpSkillList)
        {
            if (backupSkill.GetElement() == element)
            {
                atk += backupSkill.GetAttack();
                def += backupSkill.GetDefense();
                hpMaxAmount += backupSkill.GetHealth();
            }
        }

        // 初始化
        if (hpAmount < 0)
        {
            hpAmount = hpMaxAmount;
        }
        hpAmount = Math.Min(hpAmount, hpMaxAmount);

        OnHPMaxModified?.Invoke(this, EventArgs.Empty);
        Debug.Log("Atk: " + atk + "; Def: " + def + "; HP: " + hpMaxAmount);
    }

    public Skill GetLastCastSkill()
    {
        return lastCastSkill;
    }

    public int GetDamageTaken()
    {
        return damageTaken;
    }

    public int GetLastAttackDamage()
    {
        return lastAttackDamage;
    }

    public void ModifyAttackDamage(float modifyPercentage)
    {
        attackDamageMin = (int)(attackDamageMin * (1 + modifyPercentage));
        attackDamageMax = (int)(attackDamageMax * (1 + modifyPercentage));

        Debug.Log("Attack Damage Modified " + modifyPercentage);
    }

    public int GetExtraHp()
    {
        return extraHp;
    }

    public int GetExtraHpMax()
    {
        return extraHpMax;
    }

    public void SetExtraHp(int extraHpMax)
    {
        this.extraHpMax = extraHpMax;
        extraHp = extraHpMax;
    }

    public void ModifyHPMaxAmount(int modifiedAmount)
    {
        hpMaxAmount = modifiedAmount;
        hpAmount = Math.Min(hpAmount, hpMaxAmount);
        OnHPMaxModified?.Invoke(this, EventArgs.Empty);
    }

    public int GetAttackBaseDamage()
    {
        return (attackDamageMin + attackDamageMax) / 2;
    }

    public void AppendDamage(int damageAmount)
    {
        attackDamageMin += damageAmount;
        attackDamageMax += damageAmount;
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

        UpdateAttribute();
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

    public void ModifyDamageTaken(float modifyPercentage)
    {
        Debug.Log("Original Damage: " + damageTaken + "; Modified Damage: " + (int)(damageTaken * (1 + modifyPercentage)));

        damageTaken = (int)(damageTaken * (1 + modifyPercentage));
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

    // 连段攻击时，之后的攻击伤害会被修改
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

        UpdateAttribute();
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

        UpdateAttribute();
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

        UpdateAttribute();
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

    public Transform GetTokenContainerTransform()
    {
        return tokenContainerTransform;
    }

    public Transform GetBuffContainerTransform()
    {
        return buffContainerTransform;
    }

    public Transform GetDebuffContainerTransform()
    {
        return debuffContainerTransform;
    }

    public void UpdateStatContainer()
    {
        int buffCount = buffContainerTransform.childCount;
        int debuffCount = debuffContainerTransform.childCount;
        int tokenCount = tokenContainerTransform.childCount;
        statContainerTransform.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(statIconSize * (buffCount + debuffCount + tokenCount), statIconSize);

        if (buffCount != 0)
        {
            buffContainerTransform.gameObject.SetActive(true);
            RectTransform buffContainerRectTransform = buffContainerTransform.GetComponent<RectTransform>();
            buffContainerRectTransform.sizeDelta = new Vector2(statIconSize * buffCount, statIconSize);
        } else
        {
            buffContainerTransform.gameObject.SetActive(false);
        }

        if (debuffCount != 0)
        {
            debuffContainerTransform.gameObject.SetActive(true);
            RectTransform debuffContainerRectTransform = debuffContainerTransform.GetComponent<RectTransform>();
            debuffContainerRectTransform.sizeDelta = new Vector2(statIconSize * debuffCount, statIconSize);
        } else
        {
            debuffContainerTransform.gameObject.SetActive(false);
        }

        if (tokenCount != 0)
        {
            tokenContainerTransform.gameObject.SetActive(true);
            RectTransform tokenContainerRectTransform = tokenContainerTransform.GetComponent<RectTransform>();
            tokenContainerRectTransform.sizeDelta = new Vector2(statIconSize * tokenCount, statIconSize);
        } else
        {
            tokenContainerTransform.gameObject.SetActive(false);
        }
    }

    public Token SetToken(Transform tokenPrefab, int count)
    {
        tokenContainerTransform.gameObject.SetActive(true);

        Transform tokenTransform = Instantiate(tokenPrefab, tokenContainerTransform);
        Token token = tokenTransform.GetComponent<Token>();
        token.Initialize(this, count);

        UpdateStatContainer();

        return token;
    }

    public Buff SetBuff(Transform buffPrefab, int countdownMax, float setBuffTimerMax)
    {
        buffContainerTransform.gameObject.SetActive(true);

        Transform buffTransform = Instantiate(buffPrefab, buffContainerTransform);
        Buff buff = buffTransform.GetComponent<Buff>();
        buff.Initialize(this, countdownMax, setBuffTimerMax);

        UpdateStatContainer();

        return buff;
    }

    public Debuff SetDebuff(Transform debuffPrefab, int countdownMax, float setDebuffTimerMax, int extraCountdown = 0)
    {
        debuffContainerTransform.gameObject.SetActive(true);

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

        UpdateStatContainer();

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

    public void SetHealAmount(int healAmount)
    {
        this.healAmount = healAmount;
    }

    public void Heal(int healAmount)
    {
        this.healAmount = healAmount;
        OnStartHeal?.Invoke(this, healAmount);

        hpAmount += this.healAmount;
        hpAmount = Math.Min(hpAmount, hpMaxAmount);

        OnEndHeal?.Invoke(this, EventArgs.Empty);
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
        // int backupSkillCount = Math.Min(allSkillList.Count - equippedSkillCountMax, backupSkillCountMax);
        int backupSkillCount = 1;
        for (int i = 0; i < backupSkillCount; i++)
        {
            Skill backupSkill = Instantiate(allSkillList[i], backupSkillsTransform);
            backUpSkillList.Add(backupSkill);
        }

        UpdateAttribute();
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

        // Check damage modify
        OnAttacked?.Invoke(this, new ISkillCaster.OnAttackedEventArgs { 
            damage = damage,
            isRealDamage = isRealDamageTaken
        });

        if (!isRealDamageTaken)
        {
            if (damageTaken - extraHp > 0)
            {
                hpAmount -= (damageTaken - extraHp);
                hpAmount = Math.Max(0, hpAmount);
                extraHp = 0;
            }
            else
            {
                extraHp -= damageTaken;
                extraHp = Math.Max(0, extraHp);
            }
        } else
        {
            hpAmount -= damageTaken;
            hpAmount = Math.Max(0, hpAmount);
        }

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

    public void SetCastSkill(Skill skill, float castTime)
    {
        isCastingSkill = true;
        skillCastTime = castTime;
        castSkillTimer = skillCastTime;

        lastCastSkill = skill;

        OnCastSkill?.Invoke(this, skill);
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

        OnAttackReady?.Invoke(this, EventArgs.Empty);
    }

    public void QuitBattle()
    {
        attackCount = 1;

        atk = atkBeforeBattle;
        def = defBeforeBattle;

        OnQuitBattle?.Invoke(this, EventArgs.Empty);
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

    private void ResetStat()
    {
        atkBeforeBattle = atk;
        defBeforeBattle = def;

        ModifyActionPointMax(defaultActionPointMaxCount - actionPointMaxCount);
        availableActionPointCount = actionPointMaxCount;

        isAttacking = false;
        isEndingAttack = false;
        isCastingSkill = false;
        isDebuffMakingEffect = false;
        isImprisoned = false;
        attackSpeed = DEFAULT_ATTACK_SPEED;
        attackCount = 0;
        attackModifyAmount = 0;
        battlingEnemy = null;
        isRealDamage = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Enemy enemy) && !BattleManager.Instance.IsInBattle())
        {
            ResetStat();

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
