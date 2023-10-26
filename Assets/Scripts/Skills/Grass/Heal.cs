using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Skill
{
    [SerializeField] private float healPercentage = .35f;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        int healAmount = (int) (skillCaster.GetHPMaxAmount() * healPercentage);
        skillCaster.Heal(healAmount);
    }
}
