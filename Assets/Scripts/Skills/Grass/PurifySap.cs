using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurifySap : Skill
{
    [SerializeField] private int thisActionPointExpense = 2;
    [SerializeField] private float thisCastTime = 1f;

    private void Awake()
    {
        skillName = "Purify Sap";
        actionPointExpense = thisActionPointExpense;
        castTime = thisCastTime;
    }

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
