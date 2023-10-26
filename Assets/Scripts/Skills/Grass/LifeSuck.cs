using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSuck : Skill
{
    [SerializeField] private int baseDamage = 120;
    [SerializeField] private int damageDelta = 5;
    [SerializeField] private float healAmountPercentage = .5f;

    private bool isToSuckLife;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.GetOpponent().OnTakeDamage += Opponent_OnTakeDamage;

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);

        isToSuckLife = true;
    }

    private void Opponent_OnTakeDamage(object sender, int e)
    {
        if (isToSuckLife)
        {
            skillCaster.Heal((int)(e * healAmountPercentage));

            Debug.Log("Damage: " + e + "; Heal Amount: " + (int)(e * healAmountPercentage));

            isToSuckLife = false;
        }
    }
}
