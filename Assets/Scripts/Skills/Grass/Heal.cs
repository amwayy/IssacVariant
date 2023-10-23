using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Skill
{
    [SerializeField] private int thisActionPointExpense = 1;
    [SerializeField] private float thisCastTime = 1f;
    [SerializeField] private float healPercentage = .35f;

    private void Awake()
    {
        skillName = "Heal";
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        int healAmount = (int) (skillCaster.GetHPMaxAmount() * healPercentage);
        skillCaster.Heal(healAmount);
    }
}
