using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetonateSeed : Skill
{
    [SerializeField] private int seedBaseDamage = 40;

    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        Detonate();
    }

    private void Detonate()
    {
        int seedTokenCount = 0;
        foreach (Transform tokenTransform in skillCaster.GetOpponent().GetTokenContainerTransform())
        {
            if (tokenTransform.TryGetComponent(out SeedToken seedToken))
            {
                seedTokenCount = seedToken.GetCount();
                Debug.Log("Seed Count: " + seedTokenCount);
                seedToken.DestroySelf();
                break;
            }
        }

        int baseDamage = seedBaseDamage * seedTokenCount;
        skillCaster.SetAttack(baseDamage - damageDelta, baseDamage + damageDelta);
    }
}
