using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningSoul : Skill
{
    //攻击力=0 ;防御力=50 ; 血量=100
    [SerializeField] private int thisActionPointExpense = 2;
    //持续两回合
    [SerializeField] private int round = 2;
    [SerializeField] private float thisCastTime = 1f;
    //每回合失去当前血量的15%
    [SerializeField] private float injuredDamagePercentage = .15f;
    [SerializeField] private int injuredCountdownMax = 2;
    [SerializeField] private int burnCountdownMax = 1;
    [SerializeField] private Transform injuredPrefab;
    [SerializeField] private Transform burnPrefab;
    [SerializeField] private float setDebuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private float setBuffTimerMax = .5f;

    private Injured injuredDebuff;
    private int injuredCountdown; 

    private void Awake()
    {
        skillName = "Burning Soul";   // 燃烧灵魂
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;

        injuredCountdown = injuredCountdownMax;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        if (skillCaster.IsPlayer())
        {
            TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;
        } else
        {
            TurnManager.Instance.OnEnterEnemyTurn += TurnManager_OnEnterEnemyTurn;
        }

        SetInjuredDebuff();
        injuredDebuff.SetDamagePercentage(injuredDamagePercentage);

        //for (int i = 0; i < 2; i++)
        //{
        //    int damage = (int)(skillCaster.GetHPAmount() * .15f);
        //    skillCaster.TakeDamage(damage);
        //}
    }

    private void TurnManager_OnEnterEnemyTurn(object sender, System.EventArgs e)
    {
        TrySetBurnBuff();
    }

    private void TurnManager_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        TrySetBurnBuff();
    }

    private void TrySetBurnBuff()
    {
        Debug.Log("Try Set Burn");

        injuredCountdown--;

        if (injuredCountdown == 0)
        {
            TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
            TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;
        }

        if (injuredCountdown == 1)
        {
            bool isBurning = false;
            Burn burn = null;
            Transform buffContainerTransform = skillCaster.GetBuffContainerTransform();
            foreach (Transform buffTransform in buffContainerTransform)
            {
                if (buffTransform.TryGetComponent(out burn))
                {
                    isBurning = true;
                    break;
                }
            }

            if (isBurning)
            {
                burn.IncreaseCountdown(burnCountdownMax);
            }
            else
            {
                Buff buff = skillCaster.SetBuff(burnPrefab, burnCountdownMax, setBuffTimerMax);
            }
        }
    }

    private void SetInjuredDebuff()
    {
        bool isInjured = false;
        Transform debuffContainerTransform = skillCaster.GetDebuffContainerTransform();
        foreach (Transform debuffTransform in debuffContainerTransform)
        {
            if (debuffTransform.TryGetComponent(out injuredDebuff))
            {
                isInjured = true;
                break;
            }
        }

        if (isInjured)
        {
            injuredDebuff.IncreaseCountdown(injuredCountdownMax);
        }
        else
        {
            Debuff debuff = skillCaster.SetDebuff(injuredPrefab, injuredCountdownMax, setDebuffTimerMax);
            injuredDebuff = debuff.GetComponent<Injured>();
        }
    }

    private void OnDestroy()
    {
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
        TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;
    }
}
