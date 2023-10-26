using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRing : Skill
{
    [SerializeField] private int baseDamage = 50;
    [SerializeField] private int damageDelta = 5;
    [SerializeField] private int igniteCountdownMax = 2; //灼烧状态 持续2回合
    [SerializeField] private int DoubleAttackCountdownMax = 1; //
    [SerializeField] private float thisCastTime = 1f;
    [SerializeField] private float igniteProbability = .25f;
    [SerializeField] private float setBuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private float setDebuffTimerMax = .5f;   // 设定Debuff的缓冲时间
    [SerializeField] private Transform bleedDebuffPrefab;
    [SerializeField] private Transform doubleAttackBuffPrefab;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);

        System.Random random = new System.Random();
        float randomNum = (float)random.NextDouble();
        if (randomNum <= igniteProbability) //灼烧
        {
            SetIgniteDebuff();
        }
        //伤害翻倍
        if (skillCaster.IsPlayer())
        {
            TurnManager.Instance.OnEnterPlayerTurn += TurnManager_OnEnterPlayerTurn;
        }
        else
        {
            TurnManager.Instance.OnEnterEnemyTurn += TurnManager_OnEnterEnemyTurn;
        }
    }
    //灼烧
    private void SetIgniteDebuff()
    {
        bool isInIgnite = false; //
        BleedFire bleed = null;
        Transform debuffContainerTransform = skillCaster.GetOpponent().GetDebuffContainerTransform();
        foreach (Transform debuffTransform in debuffContainerTransform)
        {
            if (debuffTransform.TryGetComponent(out bleed))
            {
                isInIgnite = true;
                break;
            }
        }

        if (isInIgnite)
        {
            bleed.IncreaseCountdown(igniteCountdownMax);
        }
        else
        {
            skillCaster.GetOpponent().SetDebuff(bleedDebuffPrefab, igniteCountdownMax, setDebuffTimerMax);
        }
    }

    //双倍伤害
    private void TurnManager_OnEnterEnemyTurn(object sender, System.EventArgs e)
    {
        TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;

        SetDoubleAttackBuff();
    }

    private void TurnManager_OnEnterPlayerTurn(object sender, System.EventArgs e)
    {
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;

        SetDoubleAttackBuff();
    }

    private void SetDoubleAttackBuff()
    {
        bool isInDoubleAttack = false;
        DoubleAttack doubleAttack = null;
        Transform buffContainerTransform = skillCaster.GetBuffContainerTransform();

        if (buffContainerTransform == null) return;

        foreach (Transform buffTransform in buffContainerTransform)
        {
            if (buffTransform.TryGetComponent(out doubleAttack))
            {
                isInDoubleAttack = true;
                break;
            }
        }

        if (isInDoubleAttack)
        {
            doubleAttack.IncreaseCountdown(DoubleAttackCountdownMax);
        }
        else
        {
            skillCaster.SetBuff(doubleAttackBuffPrefab, DoubleAttackCountdownMax, setBuffTimerMax);
        }
    }

    private void OnDestroy()
    {
        TurnManager.Instance.OnEnterPlayerTurn -= TurnManager_OnEnterPlayerTurn;
        TurnManager.Instance.OnEnterEnemyTurn -= TurnManager_OnEnterEnemyTurn;
    }
}
