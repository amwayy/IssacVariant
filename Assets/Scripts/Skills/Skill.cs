using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] private GameLibrary.Element element;

    protected string skillName;
    protected int actionPointExpense;
    protected int coolingCountdownMax = 1;   // 冷却回合数，暂全设为1（方便测试）
    protected float castTime;   // 技能释放的动画时间（秒）
    protected ISkillCaster skillCaster;

    public GameLibrary.Element GetElement()
    {
        return element;
    }

    public string GetSkillName ()
    {
        return skillName;
    }

    public int GetActionPointExpense()
    {
        return actionPointExpense;
    }

    public int GetCoolingCountdownMax()
    {
        return coolingCountdownMax;
    }

    virtual public void CastSkill(ISkillCaster skillCaster)
    {
        this.skillCaster = skillCaster;

        if (skillCaster.IsPlayer())
        {
            if (Player.Instance.GetAvailableActionPointCount() < actionPointExpense) return;

            Player.Instance.UseActionPoints(actionPointExpense);
        }

        skillCaster.SetCastSkill(castTime);
     }
}
