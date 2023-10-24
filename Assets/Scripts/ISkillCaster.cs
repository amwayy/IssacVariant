using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ISkillCaster
{
    private const float ENTER_BATTLE_SPEED = 15f;

    public event EventHandler<int> OnCheckShield;   // 传递伤害值
    public event EventHandler OnEndCastSkill;
    public event EventHandler<int> OnTakeDamage;
    public event EventHandler OnAttackReady;

    public void SetDamageTaken(int modifiedDamage);

    // 追加攻击
    public void AppendDamage(int damageAmount);

    // 按百分比修改攻击力
    public void SetATK(int atk);

    // 按百分比修改防御力
    public void SetDEF(int def);

    public int GetATK();

    public int GetDEF();

    public void EndTurn();

    public Transform GetBuffContainerTransform();

    public Transform GetDebuffContainerTransform();

    public void TakeDamage(int damage, bool isRealDamage = false);

    public ISkillCaster GetOpponent();

    public Buff SetBuff(Transform buffPrefab, int countdownMax, float setBuffTimerMax);

    public Debuff SetDebuff(Transform debuffPrefab, int countdownMax, float setDebuffTimerMax, int extraCountdown = 0);

    public void SetCastSkill(float castTime);

    public void EndCastSkill();

    public int GetHPMaxAmount();

    public bool IsPlayer();

    // 连段攻击时，每次攻击后对攻击伤害进行修改
    public void SetAttackModify(int modifyAmount);

    public void SetAttack(int damageMin, int damageMax, float playerAttackSpeed = ENTER_BATTLE_SPEED, int attackCount = 1, bool isRealDamage = false);

    public void Heal(int healAmount);

    public int GetHPAmount(); //wx加
}
