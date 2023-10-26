using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ISkillCaster
{
    private const float ENTER_BATTLE_SPEED = 15f;

    public event EventHandler OnEndCastSkill;
    public event EventHandler<int> OnTakeDamage;
    public event EventHandler OnAttackReady;
    public event EventHandler<OnAttackedEventArgs> OnAttacked;

    public class OnAttackedEventArgs
    {
        public int damage;
        public bool isRealDamage;
    }

    public int GetDamageTaken();

    public int GetLastAttackDamage();

    public void SetAttackDamage(int minDamage, int maxDamage);

    public void ModifyHPMaxAmount(int modifiedAmount);

    public void UpdateStatContainer();

    public void ModifyDamageTaken(float modifyPercentage);

    // 追加攻击
    public void AppendDamage(int damageAmount);

    // 按百分比修改攻击力
    public void SetATK(int atk);

    // 按百分比修改防御力
    public void SetDEF(int def);

    public int GetATK();

    public int GetDEF();

    public void EndTurn();

    public Transform GetTokenContainerTransform();

    public Transform GetBuffContainerTransform();

    public Transform GetDebuffContainerTransform();

    public void TakeDamage(int damage, bool isRealDamage = false);

    public ISkillCaster GetOpponent();

    public Token SetToken(Transform tokenPrefab, int count);

    public Buff SetBuff(Transform buffPrefab, int countdownMax, float setBuffTimerMax);

    public Debuff SetDebuff(Transform debuffPrefab, int countdownMax, float setDebuffTimerMax, int extraCountdown = 0);

    public void SetCastSkill(Skill skill, float castTime);

    public void EndCastSkill();

    public int GetHPMaxAmount();

    public bool IsPlayer();

    // 连段攻击时，每次攻击后对攻击伤害进行修改
    public void SetAttackModify(int modifyAmount);

    public void SetAttack(int damageMin, int damageMax, float playerAttackSpeed = ENTER_BATTLE_SPEED, int attackCount = 1, bool isRealDamage = false);

    public void Heal(int healAmount);

    public int GetHPAmount(); //wx加
}
