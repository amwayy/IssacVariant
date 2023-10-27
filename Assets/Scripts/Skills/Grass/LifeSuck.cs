using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSuck : Skill
{
    [SerializeField] private float healAmountPercentage = .5f;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        skillCaster.GetOpponent().OnTakeDamage += Opponent_OnTakeDamage;

        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);
    }

    private void Opponent_OnTakeDamage(object sender, int e)
    {
        skillCaster.GetOpponent().OnTakeDamage -= Opponent_OnTakeDamage;

        skillCaster.Heal((int)(e * healAmountPercentage));

        Debug.Log("Damage: " + e + "; Heal Amount: " + (int)(e * healAmountPercentage));
    }

    private void OnDestroy()
    {
        if (skillCaster != null)
        {
            skillCaster.GetOpponent().OnTakeDamage -= Opponent_OnTakeDamage;
        }
    }
}
