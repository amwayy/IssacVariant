using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseIt : Skill
{
    [SerializeField] private int thisActionPointExpense = 2;
    [SerializeField] private float thisCastTime = 1f;

    private void Awake()
    {
        skillName = "Reverse It";   // Äæ×ªÇ¬À¤
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

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
