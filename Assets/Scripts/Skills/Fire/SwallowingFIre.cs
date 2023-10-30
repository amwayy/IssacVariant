using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwallowingFIre :Skill
{
    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);
        //清除自身负能量状态
        Transform debuffContainerTransform = skillCaster.GetDebuffContainerTransform();
        foreach (Transform debuffTransform in debuffContainerTransform)
        {
            debuffTransform.GetComponent<Debuff>().DestroySelf();
        }
        
        Transform buffsTransform = skillCaster.GetOpponent().GetBuffContainerTransform();
        Transform mineBuffTransform = skillCaster.GetBuffContainerTransform();
        foreach (Transform buffTransform in buffsTransform)
        {
            buffTransform.SetParent(mineBuffTransform);
            buffTransform.GetComponent<Buff>().SetOwner(skillCaster.GetOpponent());
        }
    }
}
