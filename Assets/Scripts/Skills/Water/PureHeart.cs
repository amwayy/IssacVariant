using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PureHeart : Skill
{
    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        Transform debuffContainerTransform = skillCaster.GetDebuffContainerTransform();
        foreach (Transform debuffTransform in debuffContainerTransform)
        {
            debuffTransform.GetComponent<Debuff>().DestroySelf();
        }
    }
}
