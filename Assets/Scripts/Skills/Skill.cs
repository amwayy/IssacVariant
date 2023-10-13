using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    protected string skillName;
    protected int actionPointExpense;
    protected float castTime;   // 技能释放的动画时间（秒）

    virtual public string GetSkillName ()
    {
        return skillName;
    }

    virtual public void CastSkill(ISkillCaster skillCaster)
    {
        if (skillCaster.IsPlayer())
        {
            if (Player.Instance.GetAvailableActionPointCount() < actionPointExpense) return;

            Player.Instance.UseActionPoints(actionPointExpense);
        }

        skillCaster.SetCastSkill(castTime);
     }
}
