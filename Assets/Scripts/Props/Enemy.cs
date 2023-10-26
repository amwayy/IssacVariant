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
    [SerializeField] private Transform debuffContainerTransform;
    [SerializeField] private Transform buffContainerTransform;
    [SerializeField] private Vector3 attackPosBias;
    [SerializeField] private int atk = 100;
    [SerializeField] private int def = 100;
    [SerializeField] GameLibrary.Element element;
    [SerializeField] private Transform statContainerTransform;
    [SerializeField] private Transform tokenContainerTransform;

    public event EventHandler<int> OnTakeDamage;
    public event EventHandler OnHeal;
    public event EventHandler OnEndCastSkill;
    public event EventHandler OnAttackReady;
    public event EventHandler<ISkillCaster.OnAttackedEventArgs> OnAttacked;
    public event EventHandler OnHPMaxModified;
    public event EventHandler<Skill> OnCastSkill;

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
    private int lastAttackDamage;
    private int attackModifyAmount;
    private List<Skill> equippedSkillList = new List<Skill>();
    private bool isRealDamage;   // 受到的是否为真实伤害，即是否无视护盾
    private int damageTaken;
    private float statIconSize = .7f;
    private Skill lastCastSkill;

    private void Awake()
    {
        ChangeDir();
        enemyChangeDirTimer = enemyChangeDirTimerMax;

        enemyRigidbody = GetComponent<Rigidbody2D>();

        hpAmount = hpMaxAmount;
        attackSpeed = DEFAULT_ATTACK_SPEED;

        buffContainerTransform.gameObject.SetActive(false);
        debuffContainerTransform.gameObject.SetActive(false);
        tokenContainerTransform.gameObject.SetActive(false);
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
    }

    public void ModifyHPMaxAmount(int modifiedAmount)
    {
        hpMaxAmount = modifiedAmount;
        hpAmount = Math.Min(hpAmount, hpMaxAmount);
        OnHPMaxModified?.Invoke(this, EventArgs.Empty);
    }

    public void AppendDamage(int damageAmount)
    {
        attackDamageMin += damageAmount;
        attackDamageMax += damageAmount;
    }

    public void ModifyDamageTaken(float modifyPercentage)
    {
        Debug.Log("Original Damage: " + damageTaken + "; Modified Damage: " + (int)(damageTaken * (1 + modifyPercentage)));

        damageTaken = (int)(damageTaken * (1 + modifyPercentage));
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
        Skill randomSkill = equippedSkillList[castedSkillIndex].GetComponent<Skill>();
        randomSkill.CastSkill(this);

        Debug.Log("Enemy Casted " + equippedSkillList[castedSkillIndex].GetSkillName());
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
        }
        else
        {
            buffContainerTransform.gameObject.SetActive(false);
        }

        if (debuffCount != 0)
        {
            debuffContainerTransform.gameObject.SetActive(true);
            RectTransform debuffContainerRectTransform = debuffContainerTransform.GetComponent<RectTransform>();
            debuffContainerRectTransform.sizeDelta = new Vector2(statIconSize * debuffCount, statIconSize);
        }
        else
        {
            debuffContainerTransform.gameObject.SetActive(false);
        }

        if (tokenCount != 0)
        {
            tokenContainerTransform.gameObject.SetActive(true);
            RectTransform tokenContainerRectTransform = tokenContainerTransform.GetComponent<RectTransform>();
            tokenContainerRectTransform.sizeDelta = new Vector2(statIconSize * tokenCount, statIconSize);
        }
        else
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
        return Player.Instance;
    }

    public void SetCastSkill(Skill skill, float castTime)
    {
        skillCastTime = castTime;
        castSkillTimer = skillCastTime;

        lastCastSkill = skill;

        OnCastSkill?.Invoke(this, skill);
    }

    public void EndCastSkill()
    {
        Debug.Log("Enemy End Cast");
        isCastingSkill = false;

        isRealDamage = false;

        OnEndCastSkill?.Invoke(this, EventArgs.Empty);

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
        List<Skill> skillList = new List<Skill>();
        foreach (Skill elementSkill in GameLibrary.Instance.GetElementSkillList(element))
        {
            if (elementSkill.IsEnemyUnappliable()) continue;

            skillList.Add(elementSkill);
        }
        for (int i = 0; i < equippedSkillCountMax; i++)
        {
            Skill randomSkill = skillList[UnityEngine.Random.Range(0, skillList.Count)];
            skillList.Remove(randomSkill);
            Skill skill = Instantiate(randomSkill, equippedSkillsTransform);
            equippedSkillList.Add(skill);
        }
    }

    public void SetAttack(int damageMin, int damageMax, float playerAttackSpeed = DEFAULT_ATTACK_SPEED, int attackCount = 1, bool isRealDamage = false)
    {
        isAttacking = true;
        attackDamageMin = (int)((float)damageMin * atk / GetOpponent().GetDEF());
        attackDamageMax = (int)((float)damageMax * atk / GetOpponent().GetDEF());
        attackSpeed = playerAttackSpeed;
        this.attackCount = attackCount;
        this.isRealDamage = isRealDamage;

        OnAttackReady?.Invoke(this, EventArgs.Empty);
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
                lastAttackDamage = attackDamage;
                Player.Instance.TakeDamage(attackDamage, isRealDamage);

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

                    if (attackModifyAmount > 0)
                    {
                        attackDamageMin = lastAttackDamage + attackModifyAmount;
                        attackDamageMax = lastAttackDamage + attackModifyAmount;
                    }
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

    public void TakeDamage(int damage, bool isRealDamageTaken = false)
    {
        damageTaken = damage;

        // Check damage modify
        OnAttacked?.Invoke(this, new ISkillCaster.OnAttackedEventArgs
        {
            damage = damage,
            isRealDamage = isRealDamageTaken
        });

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
        Player.Instance.QuitBattle();
        Player.Instance.SetLootSkillList(equippedSkillList);

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
