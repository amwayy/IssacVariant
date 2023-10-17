using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLibrary : MonoBehaviour
{
    // 储存游戏中各种数据，如技能、Buff、Debuff等

    [SerializeField] private List<Skill> allSkillList;

    public static GameLibrary Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public List<Skill> getAllSkillList()
    {
        return allSkillList;
    }
}
