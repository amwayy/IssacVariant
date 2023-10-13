using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillCaster
{
    public bool IsPlayer();

    public void SetAttack(int damageMin, int damageMax, float playerAttackSpeed, int attackCount = 1);
}
