using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseIt : Skill
{
    public override void CastSkill(ISkillCaster skillCaster)
    {
        base.CastSkill(skillCaster);

        Transform debuffsTransform = skillCaster.GetDebuffContainerTransform();
        Transform opponentDebuffTransform = skillCaster.GetOpponent().GetDebuffContainerTransform();
        foreach (Transform debuffTransform in debuffsTransform)
        {
            debuffTransform.SetParent(opponentDebuffTransform);
            debuffTransform.GetComponent<Debuff>().SetOwner(skillCaster.GetOpponent());
        }
    }
}
