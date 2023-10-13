using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedBullet : Skill
{
    [SerializeField] private int thisActionPointExpense = 1;
    [SerializeField] private int singleDamageMin = 15;
    [SerializeField] private int singleDamageMax = 25;
    [SerializeField] private int minAttackCount = 2;
    [SerializeField] private int maxAttackCount = 4;
    [SerializeField] private float attackSpeed = 20f;

    private void Awake()
    {
        skillName = "Seed Bullet";
        actionPointExpense = thisActionPointExpense;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        int randomAttckCount = Random.Range(minAttackCount, maxAttackCount + 1);

        skillCaster.SetAttack(singleDamageMin, singleDamageMax, attackSpeed, randomAttckCount);

        if (skillCaster.IsPlayer())
        {
            Player.Instance.CastSkill(actionPointExpense);
        }
    }
}
