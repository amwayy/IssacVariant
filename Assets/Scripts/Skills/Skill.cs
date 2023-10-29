using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] private GameLibrary.Element element;
    [SerializeField] private bool isEnemyUnappliable = false;   // enemy是否可以具有这个技能
    [SerializeField] private List<SkillType> skillTypeList;
    [SerializeField] protected string skillName;
    [SerializeField] private string skillDescription;
    [SerializeField] protected int actionPointExpense = 1;
    [SerializeField] protected int coolingCountdownMax = 1;   // 冷却回合数，暂全设为1（方便测试）
    [SerializeField] protected float castTime = 1f;   // 技能释放的动画时间（秒）
    [SerializeField] protected int baseDamage;   // 攻击力
    [SerializeField] protected int damageDelta = 5;   // 攻击伤害随机范围
    [SerializeField] private int defense;   // 防御力
    [SerializeField] private int health;   // 生命值

    protected ISkillCaster skillCaster;

    private void Awake()
    {
        coolingCountdownMax = actionPointExpense;
    }

    public enum SkillType
    {
        Attack,
        Buff,
        Debuff,
    }

    public int GetAttack()
    {
        return baseDamage;
    }

    public int GetDefense()
    {
        return defense;
    }

    public int GetHealth()
    {
        return health;
    }

    public string GetSkillDescription()
    {
        return skillDescription;
    }

    public List<SkillType> GetSkillTypeList()
    {
        return skillTypeList;
    }

    public bool IsEnemyUnappliable()
    {
        return isEnemyUnappliable;
    }

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

        skillCaster.SetCastSkill(this, castTime);
     }
}
