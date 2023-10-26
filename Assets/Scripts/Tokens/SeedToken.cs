using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedToken : Token
{
    [SerializeField] private float singleDamageIncreasePercentage = .05f;

    public override void Initialize(ISkillCaster skillCaster, int count)
    {
        base.Initialize(skillCaster, count);

        tokenOwner.OnAttacked += TokenOwner_OnAttacked;
    }

    private void TokenOwner_OnAttacked(object sender, ISkillCaster.OnAttackedEventArgs e)
    {
        tokenOwner.ModifyDamageTaken(singleDamageIncreasePercentage * count);
    }

    private void OnDestroy()
    {
        tokenOwner.OnAttacked -= TokenOwner_OnAttacked;
    }
}
