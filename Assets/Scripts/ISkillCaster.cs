using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillCaster
{
    public void SetCastSkill(float castTime);

    public void EndCastSkill();

    public int GetHPMaxAmount();

    public bool IsPlayer();

    public void SetAttack(int damageMin, int damageMax, float playerAttackSpeed, int attackCount = 1);

    public void Heal(int healAmount);
}
