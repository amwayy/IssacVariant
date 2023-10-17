using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillCaster
{
    private const float ENTER_BATTLE_SPEED = 15f;

    public void EndTurn();

    public Transform GetDebuffContainerTransform();

    public void TakeDamage(int damage);

    public ISkillCaster GetOpponent();

    public void SetDebuff(Transform debuffPrefab, int countdownMax, float setDebuffTimerMax);

    public void SetCastSkill(float castTime);

    public void EndCastSkill();

    public int GetHPMaxAmount();

    public bool IsPlayer();

    public void SetAttack(int damageMin, int damageMax, float playerAttackSpeed = ENTER_BATTLE_SPEED, int attackCount = 1);

    public void Heal(int healAmount);
}
