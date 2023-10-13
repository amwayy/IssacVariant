using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    protected string skillName;
    protected int actionPointExpense;

    virtual public string GetSkillName ()
    {
        return skillName;
    }

    virtual public void CastSkill(ISkillCaster skillCaster)
    {
        if (skillCaster.IsPlayer())
        {
            if (Player.Instance.IsCastingSkill()) return;

            if (Player.Instance.GetAvailableActionPointCount() < actionPointExpense) return;
        }
     }
}
